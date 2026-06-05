# Plan: Web Push notifications + PWA for "it's your turn"

I verified every integration point you mapped. Key confirmations from reading the code:
- `EndTurnCommandHandler` (line 84) and `EndGameCommandHandler` and `DeletePlayerCommandHandler` all publish `GameUpdatedNotification(updatedGame)`. The game's next player is `game.CurrentTurn.PlayerId`.
- MediatR multi-handler is the right hook — `GameUpdatedNotificationHandler` and `PlayerDeletedNotificationHandler` are independent `INotificationHandler<>`s; adding a second handler requires no edit to existing ones.
- `GameContext.OnModelCreating` auto-applies configs and auto-converts all `DateTime`/`DateTime?` to UTC — new entity gets this for free.
- `PlayerConnection` has `LastHeartbeat`; `Player.Connections` is the navigation. Presence-based dedupe is feasible server-side.
- `Program.cs` migrates at startup (line 206-211), binds config via `IConfiguration` (Jwt pattern), registers services around line 197-200, serves static files (line 214) and `MapFallbackToFile("index.html")` (line 244).
- `main.ts` uses standalone `bootstrapApplication` (no `app.config.ts`) — SW provider goes directly in the providers array.
- `game.effects.ts`: `joinedGame` effect at line 89, leave at 148, `turnAdvanced` chime at 594, `gameEnded` at 625. `GamesApiActions.joinedGame` is the post-join hook.
- `angular.json` assets are `["src/favicon.ico", "src/assets"]`; production config does env file replacement + `outputHashing: all`.
- Env files hold `baseUrl`/`production` only — add `vapidPublicKey`.

---

## 1. .NET Web Push library choice

**Recommended: `Lib.Net.Http.WebPush`** (Tomasz Pęczek).

- `WebPush` (web-push-csharp): de-facto port, but effectively unmaintained (~2020), drags in older `Portable.BouncyCastle` + `Newtonsoft.Json`. Works on .NET 8 but the abandoned crypto deps are a supply-chain concern.
- `Lib.Net.Http.WebPush`: actively maintained, modern .NET, integrates via `IHttpClientFactory`/`PushServiceClient`, companion `Lib.Net.Http.WebPush.Authentication` for VAPID, no Newtonsoft. Cleaner DI story that matches this repo.

Register in `Program.cs`: `builder.Services.AddHttpClient<PushServiceClient>();`. Keep the third-party type in the **Server** project behind our `IPushSender` abstraction (Application layer depends only on `IPushSender`), mirroring how `GameHubClientNotificationService` implements `IGameClientNotificationService`. Add the NuGet package to `GameManager.Server` so Domain/Application stay clean.

---

## 2. Backend work items (in order)

### 2.1 Domain entity — create `src/GameManager.Domain/Entities/PushSubscription.cs`
Model after `PlayerConnection.cs` (private setters, constructor). Fields:
- `Guid Id` (PK)
- `Guid PlayerId` (FK → Player)
- `Guid GameId` (denormalized — cheap prune of all subs on game end)
- `string Endpoint` (push service URL; natural unique key — index, **no length cap**, can exceed 2000 chars)
- `string P256dh`, `string Auth` (from `subscription.keys`)
- `DateTime CreatedDate`, `DateTime? LastNotifiedDate`
- `Touch()` method (parallels `UpdateHeartbeat()`).

### 2.2 EF config — create `src/GameManager.Persistence.Sqlite/Configurations/PushSubscriptionConfiguration.cs`
Mirror `PlayerConnectionConfiguration`: `ToTable("PushSubscriptions")`, `HasKey(Id)`, `HasIndex(Endpoint).IsUnique()`, indexes on `PlayerId` and `GameId`, required `Endpoint`/`P256dh`/`Auth` (no `HasMaxLength`), `HasOne<Player>().WithMany().HasForeignKey(t => t.PlayerId)` (one-directional, avoids touching `Player.cs`). Auto-applied by `ApplyConfigurationsFromAssembly`; UTC converter loop covers the datetimes.

### 2.3 Repository
- Create `src/GameManager.Application/Contracts/Persistence/IPushSubscriptionRepository.cs` : `IAsyncRepository<PushSubscription>` with `GetByEndpointAsync`, `GetByPlayerIdAsync`, `UpsertAsync`, `DeleteByEndpointAsync`, `DeleteByPlayerIdAsync`, `DeleteByGameIdAsync`.
- Create `src/GameManager.Persistence.Sqlite/Repositories/PushSubscriptionRepository.cs` extending `BaseRepository<PushSubscription>`; use `ExecuteDeleteAsync` for the deletes (same style as `UpdateHeartbeatAsync`’s `ExecuteUpdateAsync`).
- Modify `SqlitePersistenceServiceRegistration.cs`: add `services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();`

### 2.4 Migration (implementer runs)
`dotnet ef migrations add AddPushSubscriptions --project GameManager.Persistence.Sqlite --startup-project GameManager.Server`. Applied at startup by `db.Database.Migrate()`.

### 2.5 VAPID config / options
- Modify `appsettings.json` + `appsettings.Development.json`: add `"PushNotifications": { "VapidPublicKey", "VapidPrivateKey", "VapidSubject" }` (Jwt pattern).
- Create `src/GameManager.Server/Services/PushNotificationOptions.cs` POCO; bind in `Program.cs`: `builder.Services.Configure<PushNotificationOptions>(builder.Configuration.GetSection("PushNotifications"));`
- Keep private key out of source: user-secrets in dev, `PushNotifications__VapidPrivateKey` env var in prod.

### 2.6 PushSender
- Create `src/GameManager.Application/Contracts/IPushSender.cs` + small DTOs `PushPayload` / `PushResult` (enum Sent/Gone/Failed) in Application.
- Create `src/GameManager.Server/Services/PushSender.cs` implementing it with `Lib.Net.Http.WebPush`: inject `PushServiceClient` + `IOptions<PushNotificationOptions>`; set `DefaultAuthentication = new VapidAuthentication(pub, priv){ Subject }`; `RequestPushMessageDeliveryAsync(...)`; catch `PushServiceClientException`, map 404/410 → `Gone`, else `Failed` (log + swallow).
- Register: `AddHttpClient<PushServiceClient>()` + `AddScoped<IPushSender, PushSender>()`.

### 2.7 NEW turn-change push handler (core hook)
Create `src/GameManager.Application/Features/Games/Notifications/GameUpdated/GameUpdatedPushNotificationHandler.cs` : `INotificationHandler<GameUpdatedNotification>` — runs alongside the existing SignalR handler.

Logic:
1. `nextPlayerId = game.CurrentTurn?.PlayerId` — guard null.
2. If `game.State == Complete` → `DeleteByGameIdAsync(game.Id)` and return (covers EndGame and the last-player-deleted path).
3. **Dedupe (backend presence — primary).** Inject `IPlayerRepository`; check whether the next player has a live `PlayerConnection` with `LastHeartbeat` inside the existing heartbeat staleness window — if so, SKIP (they got the audio chime via `turnAdvanced`). **Tradeoff:** backend check = single source of truth (we already track heartbeats), simplest. SW `clients.matchAll` suppression is more robust to stale heartbeats and the "tab open on a different game" case but duplicates logic and still costs a round-trip. **Do both:** backend skip is primary, SW focused-client check is a safety net for heartbeat lag.
4. `GetByPlayerIdAsync(nextPlayerId)`; for each sub, send `PushPayload { Title="It's your turn", Body=game name, Url="/game", Tag="turn-"+gameId }`.
5. On `Gone` → `DeleteByEndpointAsync` (satisfies 404/410 cleanup).
6. Whole thing in try/catch + logging — must never bubble into the turn command (awaited in `_mediator.Publish`).

### 2.8 Register/Unregister endpoints + commands
Create `src/GameManager.Server/Endpoints/PushGroup.cs` (`Configure("Push", ...)`).
- Create `Endpoints/Push/RegisterSubscription/RegisterSubscriptionEndpoint.cs`: `Post("Subscribe"); Group<PushGroup>(); Version(1);` — **require auth** (read playerId/gameId from `IUserContext.User.GetPlayerId()`, never trust client ids). DTO `{ Endpoint, P256dh, Auth }` matches `PushSubscription.toJSON()`. Sends `RegisterPushSubscriptionCommand`.
- Create `src/GameManager.Application/Features/Push/Commands/RegisterSubscription/` command + handler → `UpsertAsync`.
- Create `Endpoints/Push/UnregisterSubscription/UnregisterSubscriptionEndpoint.cs` (`Post("Unsubscribe")` with `{ Endpoint }`) → `UnregisterPushSubscriptionCommand` → `DeleteByEndpointAsync`.
- Public VAPID key ships via Angular env, not a GET endpoint.

### 2.9 Cleanup hooks
- **Send failure 404/410:** in 2.7 step 5.
- **Player leave/delete:** create `src/GameManager.Application/Features/Games/Notifications/PlayerDeleted/PlayerDeletedPushCleanupHandler.cs` (second handler on `PlayerDeletedNotification`) → `DeleteByPlayerIdAsync`. Confirmed `PlayerDeletedNotification` exposes `PlayerId`.
- **Game end:** handled inside 2.7 step 2 (`GameState.Complete` — matches `g.Complete()` and the FE `state === 'Complete'` filter), covering both EndGame and last-player-deleted.

---

## 3. Frontend work items (in order)

### 3.1 Install
`cd web && npm install @angular/service-worker --save` (manual config preferred over `ng add @angular/pwa` to avoid Material theme scaffolding; trim if you use `ng add`). Match Angular 20.

### 3.2 ngsw-config.json + manifest + icons
- Create `web/ngsw-config.json` (app shell + assets caching).
- Create `web/src/manifest.webmanifest`: `name`, `short_name`, `start_url:"."`, `display:"standalone"`, `theme_color`, `background_color`, `icons` (192, 512, 512-maskable).
- Create icons under `web/src/assets/icons/` (192, 512, 512-maskable, apple-touch-icon 180).
- Modify `web/src/index.html`: add manifest link, theme-color meta, apple-touch-icon link.

### 3.3 angular.json
Modify build options: add `"manifest.webmanifest"` to `assets`; add `"serviceWorker": "ngsw-config.json"` **under `configurations.production`** so dev `ng serve` isn't cached (production-only requirement).

### 3.4 Register SW (production only)
Modify `web/src/main.ts` providers (standalone bootstrap, no app.config):
```
provideServiceWorker('custom-service-worker.js', {
  enabled: environment.production,
  registrationStrategy: 'registerWhenStable:30000',
})
```

### 3.5 Custom SW push handling — key decision
ngsw's built-in `push`/`notificationclick` is opinionated (fixed payload shape, routes clicks through its own `onActionClick`) and can't express `clients.matchAll` suppression. **Recommended: composite SW.** Create `web/src/custom-service-worker.js` (added to assets) that `importScripts('./ngsw-worker.js')` for PWA caching, then adds our own listeners. Register it (3.4) instead of `ngsw-worker.js`.
- `push`: `event.data.json()` → `{ title, body, url, tag }`; `event.waitUntil(clients.matchAll({type:'window', includeUncontrolled:true}).then(cs => cs.some(c=>c.focused) ? undefined : registration.showNotification(title, {body, tag, data:{url}, icon, badge})))` — the SW-side dedupe net.
- `notificationclick`: `notification.close()`, focus an existing same-origin window or `clients.openWindow(url)`.
- Verify `importScripts` path resolution in `dist/game-manager`. **Fallback:** ship custom SW only (no ngsw) — installability + push still work, lose ngsw caching. Document the choice.

### 3.6 PushSubscriptionService
Create `web/src/app/game/services/push-subscription.service.ts` (`providedIn:'root'`): `isSupported()`, `permissionState()`, `requestPermissionAndSubscribe()` (`serviceWorker.ready` → `Notification.requestPermission()` from a user gesture → `reg.pushManager.subscribe({userVisibleOnly:true, applicationServerKey: urlBase64ToUint8Array(environment.vapidPublicKey)})` → POST `sub.toJSON()`), `unsubscribe()` (POST endpoint to backend then `sub.unsubscribe()`), plus the `urlBase64ToUint8Array` helper.
Modify `web/src/app/game/services/game.service.ts`: add `subscribePush`/`unsubscribePush` using `this.apiUrl('Push/Subscribe'|'Push/Unsubscribe')` (JWT interceptor supplies auth → backend reads claims).

### 3.7 NgRx hooks
Modify `web/src/app/game/state/game.effects.ts`:
- New effect on `GamesApiActions.joinedGame`: do NOT auto-prompt; if `Notification.permission === 'granted'`, silently re-subscribe so the backend maps the browser sub to the new PlayerId.
- `dispatch:false` effects on `GamesApiActions.leftGame` (~148) and `GameHubActions.gameEnded` (~625): call `pushSubscriptionService.unsubscribe()` to tear down the local sub.

### 3.8 Env + toggle UI
- Modify `environment.ts` + `environment.production.ts`: add `vapidPublicKey` (public key safe to ship).
- Add an "Enable turn notifications" toggle on the in-game/lobby page (where audio/player controls live): shows Off/On/Blocked; enable → `requestPermissionAndSubscribe()` (user gesture); disable → `unsubscribe()`; if `denied`, show re-enable guidance; hide if `!isSupported()`.

---

## 4. iOS PWA caveat + install UX

- **iOS Safari** (16.4+) delivers Web Push only to **installed PWAs**; no `beforeinstallprompt`, no programmatic install. Detect iOS Safari + non-standalone (`navigator.standalone === false` / `matchMedia('(display-mode: standalone)')`); when the toggle is tapped on iOS-not-installed, show an "Add to Home Screen" instruction dialog instead of calling `requestPermission` (which no-ops in a tab).
- **Android/desktop Chromium:** capture `beforeinstallprompt` (`e.preventDefault(); deferredPrompt = e;`), show an "Install app" button, call `deferredPrompt.prompt()` on click. Install is encouraged, not required, off iOS.
- **v1 scope:** iOS instruction dialog + basic `beforeinstallprompt` capture/button. Fancy banners are follow-up.

---

## 5. VAPID key generation + docs

- Generate once per environment: `npx web-push generate-vapid-keys` or `VapidHelper.GenerateVapidKeys()` from the .NET lib.
- Private key → secret (user-secrets dev, `PushNotifications__VapidPrivateKey` env in prod). Public key → both Angular `environment*.ts` and server config; **they must match**.
- Document in README beside the existing JWT `openssl` step: a "Push notifications (VAPID)" subsection — generate command, where each key goes, the public-key-must-match note, and that rotating keys invalidates all existing subscriptions. Consider a startup log of the configured public key (first/last chars) for sanity.

---

## 6. End-to-end verification plan

**Local/manual:**
- Secure context: `http://localhost` is exempt. Since SW is production-only, either temporarily flip `enabled:true` or (preferred) `ng build --configuration production` and serve `dist/game-manager` via `npx http-server` on localhost, pointing `baseUrl` at the API.
- Two browsers: A joins as P1, B joins as P2; B enables notifications (verify 200 from `Push/Subscribe` + a `PushSubscriptions` row); close B's tab; in A end P1's turn; confirm B gets a system notification with tab closed.
- Dedupe: keep B foregrounded, end turn → no system notification (chime only).
- DevTools Application panel: Service Workers registered/active, Manifest installable, use the "Push" test payload to fire the SW handler; inspect `pushManager.getSubscription()`.
- Cleanup: player leaves → rows for that player deleted; end game → all rows for GameId deleted; corrupt an endpoint → ending a turn prunes the 404/410 row.
- iOS: physical iOS 16.4+ device, Add to Home Screen, open from icon, enable, background, trigger turn.

**Automated (`src/GameManager.Tests`, has `Commands/`, `Domain/`, `Queries/`):**
- `Domain/PushSubscriptionTests.cs` — construction / `Touch()`.
- `Commands/RegisterPushSubscriptionCommandTests.cs` — upsert (new vs existing endpoint), ids from user context.
- Handler test for `GameUpdatedPushNotificationHandler` with a fake `IPushSender` + in-memory repo (mirror `EndTurnCommandTests`/fixture): asserts push to next player, skip on live connection, `Gone` prunes, Completed game prunes by GameId and sends nothing.
- Cleanup test for `PlayerDeletedPushCleanupHandler`.
- Repository tests for the new repo. The real `Lib.Net.Http.WebPush` client is never exercised — only `IPushSender` is faked.

---

## 7. Risks, sequencing, follow-ups

**Sequencing:** (1) entity+config+migration+repo+DI → (2) options+PushSender → (3) endpoints+commands → (4) push handler + cleanup handlers; frontend (5) SW/manifest/config → (6) service+env+toggle+effects → (7) iOS/install → (8) tests. Backend 1-4 and frontend 5 can run in parallel once the 2.8 DTO contract is fixed.

**Risks:**
- **ngsw + custom SW composition (3.5)** is highest-risk (`importScripts` ordering/scope). Have the custom-SW-only fallback ready.
- **Heartbeat staleness threshold** for dedupe — reuse the existing window (confirm in `GamePruningService`/heartbeat). Too long → just-closed tab misses notify; too short → double-notify (SW check mitigates).
- **VAPID key mismatch** silently breaks delivery — README warning + startup log.
- **Endpoint length** — no `HasMaxLength` or long FCM endpoints corrupt.
- **`GameUpdatedNotification` fires on non-turn updates too** (score changes). The handler keys off `game.CurrentTurn.PlayerId` + state; if it ever fires without a turn change you could over-notify — possible follow-up: thread a "turn actually changed" flag (compare previous vs current turn). Worth a guard.

**Follow-ups (out of v1):** notification action buttons + click deep-linking to the specific game; per-player server-side preferences; other event types (explicitly out of scope); install banner polish; `pushsubscriptionchange` SW re-subscribe handler (recommended soon after v1 for reliability).

---

### Critical Files for Implementation
- /Users/kevin/git/game-manager/src/GameManager.Application/Features/Games/Notifications/GameUpdated/GameUpdatedNotificationHandler.cs (pattern for the new push handler)
- /Users/kevin/git/game-manager/src/GameManager.Persistence.Sqlite/Configurations/PlayerConnectionConfiguration.cs (pattern for PushSubscriptionConfiguration)
- /Users/kevin/git/game-manager/src/GameManager.Server/Program.cs (DI registration, migration, options binding)
- /Users/kevin/git/game-manager/web/src/main.ts (service worker registration in standalone bootstrap)
- /Users/kevin/git/game-manager/web/src/app/game/state/game.effects.ts (join/leave/turn hooks for subscribe/unsubscribe)