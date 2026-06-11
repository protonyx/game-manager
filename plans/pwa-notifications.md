# Plan: Web Push notifications + PWA for "it's your turn"

## Context

Game Manager promises that players "receive notifications when it is their turn," but today that cue is **in-app only**: a SignalR `GameStateChanged` broadcast plus an audio chime (`web/src/app/game/state/game.effects.ts` → `turnAdvanced`). If a player backgrounds the tab or locks their phone, they miss their turn. This change delivers a real **Web Push** notification that fires even when the tab is closed, and makes the app an installable **PWA** (required because iOS Safari only delivers Web Push to home-screen-installed PWAs).

**Scope (v1):**
- Trigger: only "it's your turn" (turn change). No other event types.
- Full Web Push via VAPID + PWA installability (manifest + service worker).
- Subscriptions tied to the ephemeral `PlayerId` (no user accounts). Cleaned up on game end, player leave, and dead-endpoint (HTTP 404/410).
- iOS "Add to Home Screen" guidance + Android/desktop install prompt are **in scope**.

This document is self-contained: it embeds the existing code patterns to mirror so it can be executed without prior codebase familiarity.

---

## Architecture conventions (read first)

The backend is Clean Architecture with CSharpFunctionalExtensions, MediatR, FastEndpoints, EF Core (SQLite).

- **Projects:** `GameManager.Domain` (entities), `GameManager.Application` (commands/handlers/contracts), `GameManager.Persistence.Sqlite` (EF configs/repos), `GameManager.Server` (endpoints, DI, third-party integrations).
- **Layering rule:** Domain/Application must not reference the web-push NuGet package. The push library lives in `GameManager.Server` behind an `IPushSender` abstraction declared in Application — exactly how `IGameClientNotificationService` (Application) is implemented by `GameHubClientNotificationService` (Server).
- **MediatR auto-registration:** `ApplicationServiceRegistration.AddApplicationServices()` calls `RegisterServicesFromAssemblies(...Application assembly)`. **All `IRequestHandler`/`INotificationHandler` in the Application assembly are discovered automatically — new command and notification handlers need NO manual DI.** Multiple `INotificationHandler<T>` for the same notification all run.
- **EF config auto-registration:** `GameContext.OnModelCreating` calls `ApplyConfigurationsFromAssembly(...)` and auto-converts all `DateTime`/`DateTime?` to UTC. A new `IEntityTypeConfiguration<T>` is picked up automatically; datetime UTC handling is free.
- **Migrations** run at startup via `db.Database.Migrate()` in `Program.cs` (lines 206-211). Create with `dotnet ef`.
- **Routing:** FastEndpoints config (`Program.cs` ~222) sets `RoutePrefix = "api"`, version prefix `v`, default version 1, prepend-to-route. So `Post("Subscribe")` in a group configured `"Push"` → `POST /api/v1/Push/Subscribe`. Endpoints require JWT auth **unless** they call `AllowAnonymous()`.
- **Auth on the client:** `AuthInterceptorService` (registered `multi:true`) attaches `Authorization: Bearer <token>` to **every** `HttpClient` request automatically. New API calls are authenticated with no extra work; the server reads `IUserContext.User.GetPlayerId()` / `GetGameId()` from JWT claims.

### Pattern: domain entity (`PlayerConnection.cs`)
```csharp
public class PlayerConnection
{
    public Guid PlayerId { get; private set; }
    public string ConnectionId { get; private set; }
    public DateTime ConnectedDate { get; private set; }
    public DateTime LastHeartbeat { get; private set; }
    public PlayerConnection(Guid playerId, string connectionId) { ... }
    public void UpdateHeartbeat() => LastHeartbeat = DateTime.UtcNow;
}
```
`BaseRepository<T>` constrains `T : class, IEntity<Guid>` — so a repo-backed entity must implement `IEntity<Guid>` (`GameManager.Domain.Common.IEntity` = `{ TKey Id { get; } }`). EF uses constructor binding (matches ctor param names to properties); properties not in the ctor are set via their private setters on read.

### Pattern: EF config (`PlayerConnectionConfiguration.cs`)
```csharp
public class PlayerConnectionConfiguration : IEntityTypeConfiguration<PlayerConnection>
{
    public void Configure(EntityTypeBuilder<PlayerConnection> builder)
    {
        builder.ToTable("PlayerConnections");
        builder.HasKey(t => new { t.PlayerId, t.ConnectionId });
        builder.Property(t => t.ConnectionId).IsRequired().HasMaxLength(20);
        builder.HasOne<Player>().WithMany(t => t.Connections).HasForeignKey(t => t.PlayerId);
    }
}
```

### Pattern: command + handler with no return (`EndTurnCommandHandler.cs`)
Returns `UnitResult<ApplicationError>`; `ICommandHandler<TCommand> : IRequestHandler<TCommand, UnitResult<ApplicationError>>`. Success: `return UnitResult.Success<ApplicationError>();`. Mirror its `using` directives (CSharpFunctionalExtensions etc.).

### Pattern: FastEndpoints group + endpoint (`GamesGroup.cs`, `JoinGameEndpoint.cs`)
```csharp
public class GamesGroup : Group {
    public GamesGroup() => Configure("Games", ep => ep.Description(x => x.WithTags("Games")));
}
public class JoinGameEndpoint : Endpoint<JoinGameDTO, Results<Ok<PlayerCredentialsDTO>, ProblemDetails>> {
    public override void Configure() { Post("Join"); Group<GamesGroup>(); AllowAnonymous(); Version(1); }
    public override async Task<...> ExecuteAsync(JoinGameDTO req, CancellationToken ct) {
        var result = await _mediator.Send(new JoinGameCommand(...), ct);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error.ToProblemDetails();
    }
}
```

### Pattern: second notification handler (`PlayerDeletedNotificationHandler.cs`)
`PlayerDeletedNotification` exposes `GameId` and `PlayerId`. Adding another `INotificationHandler<PlayerDeletedNotification>` runs alongside the existing one.

### Verified trigger facts
- `GameUpdatedNotification(game)` is published from exactly 4 turn/state-transition paths: `StartGameCommandHandler:52`, `EndTurnCommandHandler:84`, `EndGameCommandHandler:45`, `DeletePlayerCommandHandler:97` (inside `if (game.CheckCurrentTurn(...))`). Score/tracker edits use a **separate** `PlayerTrackerUpdatedNotification` — they never fire `GameUpdated`. **Hooking push to `GameUpdatedNotification` will not over-notify.**
- `Game` (record): `Name` is a `GameName` value object (use `game.Name.Value`), `State` is `GameState` enum (`Preparing`/`InProgress`/`Complete`), `CurrentTurn` is `CurrentTurnDetails?` with `.PlayerId`. On `Complete()`, `CurrentTurn` is set to `null`.
- Frontend turn detection already exists: `turnAdvanced` effect compares `action.game.currentTurnPlayerId === playerId` (selector `selectCurrentPlayerId` from `credentials.playerId`).

---

## 1. Web-push library

**Use `Lib.Net.Http.WebPush`** (actively maintained, modern .NET, `IHttpClientFactory`-based, no Newtonsoft). Avoid `WebPush`/web-push-csharp (unmaintained since ~2020, drags in old BouncyCastle + Newtonsoft).

Add the package to **`GameManager.Server` only**:
```
dotnet add src/GameManager.Server package Lib.Net.Http.WebPush
```
> ⚠️ Verify exact type/method names against the installed version: `PushServiceClient`, `PushMessage`, `PushSubscription` (library's own type — name-clashes with our domain entity; alias one of them), `VapidAuthentication` (namespace `Lib.Net.Http.WebPush.Authentication`), and `PushServiceClientException.StatusCode`. Adjust the `PushSender` below if signatures differ.

---

## 2. Backend (in order)

### 2.1 Entity — create `src/GameManager.Domain/Entities/PushSubscription.cs`
```csharp
using GameManager.Domain.Common;

namespace GameManager.Domain.Entities;

public class PushSubscription : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid GameId { get; private set; }
    public string Endpoint { get; private set; }
    public string P256dh { get; private set; }
    public string Auth { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastNotifiedDate { get; private set; }

    public PushSubscription(Guid playerId, Guid gameId, string endpoint, string p256dh, string auth)
    {
        Id = Guid.NewGuid();
        PlayerId = playerId;
        GameId = gameId;
        Endpoint = endpoint;
        P256dh = p256dh;
        Auth = auth;
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkNotified() => LastNotifiedDate = DateTime.UtcNow;
}
```

### 2.2 EF config — create `src/GameManager.Persistence.Sqlite/Configurations/PushSubscriptionConfiguration.cs`
```csharp
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Endpoint).IsRequired();   // NO HasMaxLength — push endpoints can exceed 2000 chars
        builder.Property(t => t.P256dh).IsRequired();
        builder.Property(t => t.Auth).IsRequired();
        builder.HasIndex(t => t.Endpoint).IsUnique();
        builder.HasIndex(t => t.PlayerId);
        builder.HasIndex(t => t.GameId);
        builder.HasOne<Player>().WithMany().HasForeignKey(t => t.PlayerId);
    }
}
```

### 2.3 Repository
- Create `src/GameManager.Application/Contracts/Persistence/IPushSubscriptionRepository.cs`:
```csharp
using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface IPushSubscriptionRepository : IAsyncRepository<PushSubscription>
{
    Task<PushSubscription?> GetByEndpointAsync(string endpoint, CancellationToken ct = default);
    Task<IReadOnlyList<PushSubscription>> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task DeleteByEndpointAsync(string endpoint, CancellationToken ct = default);
    Task DeleteByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task DeleteByGameIdAsync(Guid gameId, CancellationToken ct = default);
}
```
- Create `src/GameManager.Persistence.Sqlite/Repositories/PushSubscriptionRepository.cs`:
```csharp
using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class PushSubscriptionRepository : BaseRepository<PushSubscription>, IPushSubscriptionRepository
{
    public PushSubscriptionRepository(GameContext context) : base(context) { }

    public Task<PushSubscription?> GetByEndpointAsync(string endpoint, CancellationToken ct = default)
        => _context.Set<PushSubscription>().FirstOrDefaultAsync(t => t.Endpoint == endpoint, ct);

    public async Task<IReadOnlyList<PushSubscription>> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => await _context.Set<PushSubscription>().AsNoTracking().Where(t => t.PlayerId == playerId).ToListAsync(ct);

    public Task DeleteByEndpointAsync(string endpoint, CancellationToken ct = default)
        => _context.Set<PushSubscription>().Where(t => t.Endpoint == endpoint).ExecuteDeleteAsync(ct);

    public Task DeleteByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => _context.Set<PushSubscription>().Where(t => t.PlayerId == playerId).ExecuteDeleteAsync(ct);

    public Task DeleteByGameIdAsync(Guid gameId, CancellationToken ct = default)
        => _context.Set<PushSubscription>().Where(t => t.GameId == gameId).ExecuteDeleteAsync(ct);
}
```
- Modify `src/GameManager.Persistence.Sqlite/SqlitePersistenceServiceRegistration.cs` — add to the repository block:
```csharp
services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
```

### 2.4 Presence check — add to `IPlayerRepository` + `PlayerRepository`
For dedupe. `Player.GetByIdAsync` uses `FindAsync` and does NOT load `Connections`, so query `PlayerConnection` directly (mirrors `UpdateHeartbeatAsync` which already queries `_context.Set<PlayerConnection>()`).
- `IPlayerRepository.cs`: add
```csharp
Task<bool> HasLiveConnectionAsync(Guid playerId, DateTime since, CancellationToken cancellationToken = default);
```
- `PlayerRepository.cs`: add
```csharp
public Task<bool> HasLiveConnectionAsync(Guid playerId, DateTime since, CancellationToken ct = default)
    => _context.Set<PlayerConnection>().AnyAsync(c => c.PlayerId == playerId && c.LastHeartbeat >= since, ct);
```

### 2.5 Migration
```
dotnet ef migrations add AddPushSubscriptions \
  --project src/GameManager.Persistence.Sqlite --startup-project src/GameManager.Server
```
Applied automatically at startup.

### 2.6 VAPID config + options
- `src/GameManager.Server/appsettings.json` — add a section (public key fine to commit; private key stays empty here, supplied via secret):
```json
"PushNotifications": {
  "VapidPublicKey": "",
  "VapidPrivateKey": "",
  "VapidSubject": "mailto:admin@example.com"
}
```
- Create `src/GameManager.Server/Services/PushNotificationOptions.cs`:
```csharp
namespace GameManager.Server.Services;
public class PushNotificationOptions
{
    public string VapidPublicKey { get; set; } = string.Empty;
    public string VapidPrivateKey { get; set; } = string.Empty;
    public string VapidSubject { get; set; } = "mailto:admin@example.com";
}
```
- Dev secret: `dotnet user-secrets set "PushNotifications:VapidPrivateKey" "<key>"` (run in `src/GameManager.Server`). Prod: env var `PushNotifications__VapidPrivateKey`.

### 2.7 IPushSender abstraction (Application) — create `src/GameManager.Application/Contracts/IPushSender.cs`
```csharp
namespace GameManager.Application.Contracts;

public record PushTarget(string Endpoint, string P256dh, string Auth);
public record PushPayload(string Title, string Body, string Url, string? Tag = null);
public enum PushSendResult { Sent, Gone, Failed }

public interface IPushSender
{
    Task<PushSendResult> SendAsync(PushTarget target, PushPayload payload, CancellationToken cancellationToken = default);
}
```

### 2.8 PushSender implementation (Server) — create `src/GameManager.Server/Services/PushSender.cs`
```csharp
using System.Net;
using System.Text.Json;
using GameManager.Application.Contracts;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;
using WebPushSubscription = Lib.Net.Http.WebPush.PushSubscription; // alias to avoid clash with domain entity

namespace GameManager.Server.Services;

public class PushSender : IPushSender
{
    private readonly PushServiceClient _client;
    private readonly ILogger<PushSender> _logger;

    public PushSender(PushServiceClient client, IOptions<PushNotificationOptions> options, ILogger<PushSender> logger)
    {
        _client = client;
        _logger = logger;
        var o = options.Value;
        _client.DefaultAuthentication = new VapidAuthentication(o.VapidPublicKey, o.VapidPrivateKey)
        {
            Subject = o.VapidSubject
        };
    }

    public async Task<PushSendResult> SendAsync(PushTarget target, PushPayload payload, CancellationToken ct = default)
    {
        var subscription = new WebPushSubscription
        {
            Endpoint = target.Endpoint,
            Keys = new Dictionary<string, string> { ["p256dh"] = target.P256dh, ["auth"] = target.Auth }
        };
        var json = JsonSerializer.Serialize(new { title = payload.Title, body = payload.Body, url = payload.Url, tag = payload.Tag });
        try
        {
            await _client.RequestPushMessageDeliveryAsync(subscription, new PushMessage(json), ct);
            return PushSendResult.Sent;
        }
        catch (PushServiceClientException ex) when (ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
        {
            return PushSendResult.Gone;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Push send failed for endpoint {Endpoint}", target.Endpoint);
            return PushSendResult.Failed;
        }
    }
}
```
- Register in `Program.cs` (near the other service registrations, ~line 197):
```csharp
builder.Services.Configure<PushNotificationOptions>(builder.Configuration.GetSection("PushNotifications"));
builder.Services.AddHttpClient<PushServiceClient>();
builder.Services.AddScoped<IPushSender, PushSender>();
```

### 2.9 Turn-change push handler (core hook) — create `src/GameManager.Application/Features/Games/Notifications/GameUpdated/GameUpdatedPushNotificationHandler.cs`
Runs alongside the existing `GameUpdatedNotificationHandler` (SignalR). Trigger is verified safe (see conventions) — no guard/dedicated notification needed.
```csharp
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameManager.Application.Features.Games.Notifications.GameUpdated;

public class GameUpdatedPushNotificationHandler : INotificationHandler<GameUpdatedNotification>
{
    private static readonly TimeSpan LiveConnectionWindow = TimeSpan.FromSeconds(90); // align with heartbeat/pruning window

    private readonly IPushSubscriptionRepository _subscriptions;
    private readonly IPlayerRepository _players;
    private readonly IPushSender _pushSender;
    private readonly ILogger<GameUpdatedPushNotificationHandler> _logger;

    public GameUpdatedPushNotificationHandler(
        IPushSubscriptionRepository subscriptions, IPlayerRepository players,
        IPushSender pushSender, ILogger<GameUpdatedPushNotificationHandler> logger)
    {
        _subscriptions = subscriptions; _players = players; _pushSender = pushSender; _logger = logger;
    }

    public async Task Handle(GameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var game = notification.Game;

            if (game.State == GameState.Complete)
            {
                await _subscriptions.DeleteByGameIdAsync(game.Id, cancellationToken);
                return;
            }

            var nextPlayerId = game.CurrentTurn?.PlayerId;
            if (nextPlayerId is null) return;

            // Dedupe: foregrounded player already got the in-app chime via SignalR.
            if (await _players.HasLiveConnectionAsync(nextPlayerId.Value, DateTime.UtcNow - LiveConnectionWindow, cancellationToken))
                return;

            var subs = await _subscriptions.GetByPlayerIdAsync(nextPlayerId.Value, cancellationToken);
            if (subs.Count == 0) return;

            var payload = new PushPayload("It's your turn", game.Name.Value, "/game", $"turn-{game.Id}");
            foreach (var sub in subs)
            {
                var result = await _pushSender.SendAsync(new PushTarget(sub.Endpoint, sub.P256dh, sub.Auth), payload, cancellationToken);
                if (result == PushSendResult.Gone)
                    await _subscriptions.DeleteByEndpointAsync(sub.Endpoint, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed processing push for game update"); // never bubble into the turn command
        }
    }
}
```

### 2.10 Register/Unregister endpoints + commands
**Command + handler** under `src/GameManager.Application/Features/Push/Commands/RegisterSubscription/`:
- `RegisterPushSubscriptionCommand.cs` — `ICommand` (no response) with `Guid PlayerId, Guid GameId, string Endpoint, string P256dh, string Auth`.
- `RegisterPushSubscriptionCommandHandler.cs` — `ICommandHandler<RegisterPushSubscriptionCommand>`; if `GetByEndpointAsync` returns existing → `DeleteByEndpointAsync` first (re-map to new player), then `CreateAsync(new PushSubscription(...))`; return `UnitResult.Success<ApplicationError>()`. (Mirror `EndTurnCommandHandler` usings for `UnitResult`/`ApplicationError`.)

Under `src/GameManager.Application/Features/Push/Commands/UnregisterSubscription/`:
- `UnregisterPushSubscriptionCommand.cs` — `ICommand` with `string Endpoint`.
- `UnregisterPushSubscriptionCommandHandler.cs` — calls `DeleteByEndpointAsync`.

**Endpoints** under `src/GameManager.Server/Endpoints/Push/`:
- `PushGroup.cs`:
```csharp
public class PushGroup : Group {
    public PushGroup() => Configure("Push", ep => ep.Description(x => x.WithTags("Push")));
}
```
- `RegisterSubscriptionEndpoint.cs` (require auth — no `AllowAnonymous`):
```csharp
public class RegisterSubscriptionDTO { public string Endpoint { get; set; } = ""; public string P256dh { get; set; } = ""; public string Auth { get; set; } = ""; }

public class RegisterSubscriptionEndpoint : Endpoint<RegisterSubscriptionDTO, Results<Ok, ProblemDetails>>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    public RegisterSubscriptionEndpoint(IMediator mediator, IUserContext userContext) { _mediator = mediator; _userContext = userContext; }

    public override void Configure() { Post("Subscribe"); Group<PushGroup>(); Version(1); }

    public override async Task<Results<Ok, ProblemDetails>> ExecuteAsync(RegisterSubscriptionDTO req, CancellationToken ct)
    {
        var playerId = _userContext.User?.GetPlayerId();
        var gameId = _userContext.User?.GetGameId();
        if (playerId is null || gameId is null)
            return ApplicationError.Authorization("Not authorized").ToProblemDetails();

        var result = await _mediator.Send(
            new RegisterPushSubscriptionCommand(playerId.Value, gameId.Value, req.Endpoint, req.P256dh, req.Auth), ct);
        return result.IsSuccess ? TypedResults.Ok() : result.Error.ToProblemDetails();
    }
}
```
> `GetPlayerId()`/`GetGameId()` are in `GameManager.Application.Authorization.ClaimsPrincipalExtensions`. Confirm the exact `ApplicationError.Authorization(...)` factory name against `GameErrors`/`ApplicationError` usage elsewhere; adjust if different.
- `UnregisterSubscriptionEndpoint.cs` — `Post("Unsubscribe")`, body `{ Endpoint }`, sends `UnregisterPushSubscriptionCommand`. (Trust the endpoint string; deletion is idempotent.)

The public VAPID key is shipped via the Angular env, **not** a GET endpoint.

### 2.11 Cleanup hooks (summary)
- **Dead endpoint (404/410):** handled in 2.9 (`Gone` → delete).
- **Player leaves/deleted:** create `src/GameManager.Application/Features/Games/Notifications/PlayerDeleted/PlayerDeletedPushCleanupHandler.cs`:
```csharp
public class PlayerDeletedPushCleanupHandler : INotificationHandler<PlayerDeletedNotification>
{
    private readonly IPushSubscriptionRepository _subscriptions;
    public PlayerDeletedPushCleanupHandler(IPushSubscriptionRepository subscriptions) => _subscriptions = subscriptions;
    public Task Handle(PlayerDeletedNotification notification, CancellationToken cancellationToken)
        => _subscriptions.DeleteByPlayerIdAsync(notification.PlayerId, cancellationToken);
}
```
- **Game end:** handled in 2.9 (`GameState.Complete` → `DeleteByGameIdAsync`).

---

## 3. Frontend (in order)

### 3.1 Install
```
cd web && npm install @angular/service-worker
```
(Version-match the installed Angular, currently `^20`. Prefer manual config over `ng add @angular/pwa` to avoid Material theme scaffolding.)

### 3.2 ngsw-config.json — create `web/ngsw-config.json`
```json
{
  "$schema": "./node_modules/@angular/service-worker/config/schema.json",
  "index": "/index.html",
  "assetGroups": [
    { "name": "app", "installMode": "prefetch", "updateMode": "prefetch",
      "resources": { "files": ["/favicon.ico", "/index.html", "/manifest.webmanifest", "/*.css", "/*.js"] } },
    { "name": "assets", "installMode": "lazy", "updateMode": "prefetch",
      "resources": { "files": ["/assets/**", "/*.(svg|png|jpg|webp|woff2)"] } }
  ]
}
```

### 3.3 Manifest + icons
- Create `web/src/manifest.webmanifest`:
```json
{
  "name": "Game Manager",
  "short_name": "GameMgr",
  "start_url": ".",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#1976d2",
  "icons": [
    { "src": "assets/icons/icon-192.png", "sizes": "192x192", "type": "image/png" },
    { "src": "assets/icons/icon-512.png", "sizes": "512x512", "type": "image/png" },
    { "src": "assets/icons/icon-512-maskable.png", "sizes": "512x512", "type": "image/png", "purpose": "maskable" }
  ]
}
```
- Create icons in `web/src/assets/icons/`: `icon-192.png`, `icon-512.png`, `icon-512-maskable.png`, `badge-72.png` (monochrome), and `apple-touch-icon.png` (180×180).

### 3.4 index.html — modify `web/src/index.html` `<head>`
```html
<link rel="manifest" href="manifest.webmanifest" />
<meta name="theme-color" content="#1976d2" />
<link rel="apple-touch-icon" href="assets/icons/apple-touch-icon.png" />
```

### 3.5 angular.json — modify the `build` target
- Add to `options.assets` (object form routes the custom SW to output root):
```json
"assets": [
  "src/favicon.ico",
  "src/assets",
  "src/manifest.webmanifest",
  { "glob": "custom-service-worker.js", "input": "src", "output": "/" }
]
```
- Add to `configurations.production` (production-only so dev `ng serve` isn't cached):
```json
"serviceWorker": "ngsw-config.json"
```
This makes the build emit `ngsw-worker.js` + `ngsw.json` into the output root in production.

### 3.6 Custom service worker — create `web/src/custom-service-worker.js`
Composite: keep ngsw caching, add our own push handling (ngsw's built-in push can't do focused-client suppression).
```js
importScripts('./ngsw-worker.js');

self.addEventListener('push', (event) => {
  if (!event.data) return;
  const data = event.data.json(); // { title, body, url, tag }
  event.waitUntil(
    self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clients) => {
      if (clients.some((c) => c.focused)) return; // safety-net dedupe
      return self.registration.showNotification(data.title, {
        body: data.body,
        tag: data.tag,
        data: { url: data.url },
        icon: '/assets/icons/icon-192.png',
        badge: '/assets/icons/badge-72.png',
      });
    })
  );
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  const url = (event.notification.data && event.notification.data.url) || '/';
  event.waitUntil(
    self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clients) => {
      for (const c of clients) { if ('focus' in c) { c.navigate(url); return c.focus(); } }
      return self.clients.openWindow(url);
    })
  );
});
```
> Verify `importScripts('./ngsw-worker.js')` resolves in `dist/game-manager` (both files at output root). **Fallback** if composition misbehaves: ship the custom SW only (drop the `serviceWorker` config + `importScripts`); installability + push still work, you lose ngsw asset caching.

### 3.7 Register the SW — modify `web/src/main.ts`
Add import and a provider in the `providers` array:
```ts
import { provideServiceWorker } from '@angular/service-worker';
// ...
provideServiceWorker('custom-service-worker.js', {
  enabled: environment.production,
  registrationStrategy: 'registerWhenStable:30000',
}),
```

### 3.8 Env — modify both env files
- `web/src/environments/environment.ts` and `environment.production.ts`: add `vapidPublicKey: '<public key>'` (public key is safe to ship).

### 3.9 GameService — add methods to `web/src/app/game/services/game.service.ts`
```ts
public subscribePush(sub: { endpoint: string; p256dh: string; auth: string }): Observable<void> {
  return this.http.post<void>(this.apiUrl('Push/Subscribe'), sub);
}
public unsubscribePush(endpoint: string): Observable<void> {
  return this.http.post<void>(this.apiUrl('Push/Unsubscribe'), { endpoint });
}
```
(Auth header is added automatically by `AuthInterceptorService`.)

### 3.10 PushSubscriptionService — create `web/src/app/game/services/push-subscription.service.ts`
```ts
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GameService } from './game.service';

@Injectable({ providedIn: 'root' })
export class PushSubscriptionService {
  constructor(private gameService: GameService) {}

  isSupported(): boolean {
    return 'serviceWorker' in navigator && 'PushManager' in window && 'Notification' in window;
  }

  permission(): NotificationPermission {
    return this.isSupported() ? Notification.permission : 'denied';
  }

  async requestPermissionAndSubscribe(): Promise<boolean> {
    if (!this.isSupported()) return false;
    if ((await Notification.requestPermission()) !== 'granted') return false;
    const reg = await navigator.serviceWorker.ready;
    const sub = await reg.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: this.urlBase64ToUint8Array(environment.vapidPublicKey),
    });
    const json = sub.toJSON();
    await firstValueFrom(this.gameService.subscribePush({
      endpoint: sub.endpoint,
      p256dh: json.keys!['p256dh'],
      auth: json.keys!['auth'],
    }));
    return true;
  }

  async unsubscribe(): Promise<void> {
    if (!this.isSupported()) return;
    const reg = await navigator.serviceWorker.ready;
    const sub = await reg.pushManager.getSubscription();
    if (!sub) return;
    try { await firstValueFrom(this.gameService.unsubscribePush(sub.endpoint)); } catch { /* ignore */ }
    await sub.unsubscribe();
  }

  private urlBase64ToUint8Array(base64: string): Uint8Array {
    const padding = '='.repeat((4 - (base64.length % 4)) % 4);
    const b64 = (base64 + padding).replace(/-/g, '+').replace(/_/g, '/');
    const raw = atob(b64);
    return Uint8Array.from([...raw].map((c) => c.charCodeAt(0)));
  }
}
```
> Add `vapidPublicKey` to the `environment` type if a typed interface exists; the current env files are plain objects, so just add the property.

### 3.11 NgRx effects — add to `web/src/app/game/state/game.effects.ts`
Mirror the existing functional-effect style (`inject(...)`, `createEffect(..., { functional: true, dispatch: false })`). Import `PushSubscriptionService`.
```ts
// Re-map the browser's existing subscription to the new PlayerId after joining (only if already granted; no prompt here).
export const resubscribePushOnJoin = createEffect(
  (actions$ = inject(Actions), push = inject(PushSubscriptionService)) =>
    actions$.pipe(
      ofType(GamesApiActions.joinedGame),
      tap(() => { if (push.isSupported() && push.permission() === 'granted') push.requestPermissionAndSubscribe(); }),
    ),
  { functional: true, dispatch: false },
);

export const unsubscribePushOnLeaveOrEnd = createEffect(
  (actions$ = inject(Actions), push = inject(PushSubscriptionService)) =>
    actions$.pipe(
      ofType(GamesApiActions.leftGame, GameHubActions.gameEnded),
      tap(() => push.unsubscribe()),
    ),
  { functional: true, dispatch: false },
);
```

### 3.12 Toggle + install UX (component)
Add an "Enable turn notifications" control on the in-game/lobby page (where audio/player controls already live — locate the game-page component under `web/src/app/game/`). Behavior:
- Hidden when `!push.isSupported()`.
- State display: Off / On / Blocked (`push.permission()`).
- **Enable** (must be a real user click — permission prompts require a gesture):
  - On iOS Safari **not installed** → show "Add to Home Screen" instructions dialog instead of prompting (see §4).
  - Else → `await push.requestPermissionAndSubscribe()`; if it returns false and permission is `denied`, show guidance to re-enable in browser settings.
- **Disable** → `push.unsubscribe()`.

---

## 4. iOS + install UX (in scope)

- **Detection helpers** (put in `PushSubscriptionService` or a small util):
```ts
isIos(): boolean { return /iphone|ipad|ipod/i.test(navigator.userAgent); }
isStandalone(): boolean {
  return window.matchMedia('(display-mode: standalone)').matches || (navigator as any).standalone === true;
}
```
- **iOS Safari, not installed:** Web Push is unavailable in a tab and `Notification.requestPermission()` no-ops. When the toggle is tapped in this state, show a dialog: "To get turn notifications on iPhone/iPad, tap the Share button and choose 'Add to Home Screen', then open Game Manager from your home screen." (Use the existing Angular Material dialog pattern in `web/src/app/game/dialogs/`.)
- **Android/desktop Chromium:** capture the install prompt globally (e.g., in `AppComponent`):
```ts
let deferredPrompt: any = null;
window.addEventListener('beforeinstallprompt', (e) => { e.preventDefault(); deferredPrompt = e; /* reveal an "Install app" button */ });
// on button click: deferredPrompt?.prompt();
```
  Installation is encouraged but not required off iOS. Banner polish is a follow-up.

---

## 5. VAPID key generation + docs

- Generate once per environment: `npx web-push generate-vapid-keys` (or `Lib.Net.Http.WebPush` `VapidHelper.GenerateVapidKeys()`).
- **Public key** → both `web/src/environments/environment*.ts` (`vapidPublicKey`) and server `PushNotifications:VapidPublicKey`. They MUST match.
- **Private key** → secret only (dev user-secrets, prod `PushNotifications__VapidPrivateKey`). Never commit.
- Add a "Push notifications (VAPID)" subsection to `README.md` next to the existing JWT `openssl` step: the generate command, where each key goes, the public-keys-must-match note, and that rotating keys invalidates all existing subscriptions.

---

## 6. Verification

**Local / manual** (SW is production-only):
1. Generate VAPID keys; set public key in both env + appsettings, private key in user-secrets.
2. `cd web && ng build --configuration production`; serve `dist/game-manager` on `http://localhost` (e.g. `npx http-server dist/game-manager -p 4200`). `localhost` is an exempt secure context. Run the API; point `environment.production` `baseUrl` appropriately (or serve the built SPA from the API's static files).
3. DevTools → Application: confirm Service Worker active, Manifest "installable", and use the **Push** test box to fire the SW handler.
4. Two browsers: A joins as P1, B as P2; B clicks Enable (expect `200 /api/v1/Push/Subscribe` and a `PushSubscriptions` row). **Close B's tab.** In A, end P1's turn → B receives a system notification; clicking it focuses/opens `/game`.
5. **Dedupe:** keep B foregrounded, end turn → no system notification (chime only).
6. **Cleanup:** player leaves → their rows gone; end game → all rows for that `GameId` gone; corrupt a stored endpoint → next turn-change prunes it (`Gone`).
7. **iOS:** physical iOS 16.4+ device → Add to Home Screen → open from icon → Enable → background → trigger a turn.

**Automated** (`src/GameManager.Tests`, has `Commands/`, `Domain/`, `Queries/`; mirror `EndTurnCommandTests`):
- `Domain/PushSubscriptionTests.cs` — construction + `MarkNotified()`.
- `Commands/RegisterPushSubscriptionCommandTests.cs` — new vs. existing-endpoint upsert.
- Handler test for `GameUpdatedPushNotificationHandler` with a fake `IPushSender` + in-memory repos: pushes to next player; skips when `HasLiveConnectionAsync` true; `Gone` prunes; `Complete` state prunes by `GameId` and sends nothing.
- `PlayerDeletedPushCleanupHandler` test. The real `Lib.Net.Http.WebPush` client is never exercised — only `IPushSender` is faked.

---

## 7. Sequencing & risks

**Order:** Backend 2.1→2.11, then Frontend 3.1→3.12, then 4, then 5, then 6. Backend and the frontend SW/manifest scaffolding (3.1-3.7) can proceed in parallel once the `RegisterSubscriptionDTO` shape (`{ endpoint, p256dh, auth }`) is fixed.

**Risks:**
- **Library API drift** — verify `Lib.Net.Http.WebPush` type/method/exception names against the installed version (§1); the `PushSubscription` name clash is handled via the `WebPushSubscription` alias.
- **Composite SW** (`importScripts`) is the highest-risk frontend piece; custom-SW-only fallback documented in 3.6.
- **Push sent in the request path** — the handler runs inside `_mediator.Publish` awaited by the turn command, so push HTTP calls add latency to `EndTurn`. The try/catch prevents failures from breaking the turn; if latency becomes an issue, move sends to a background queue/hosted service (follow-up).
- **Dedupe window (90s)** — align `LiveConnectionWindow` with the real heartbeat/pruning threshold (check `GamePruningService` + the 30s client heartbeat). Too long → just-closed tab misses a push; too short → double cue (SW focused-client check mitigates).
- **VAPID mismatch** silently breaks delivery — README warning; optionally log the configured public key prefix at startup.
- `game.Name.Value` — confirm `GameName` exposes `.Value` (other value objects do); else use `.ToString()`.

**Follow-ups (out of v1):** notification action buttons + deep-link to the specific game; `pushsubscriptionchange` SW re-subscribe handler; server-side per-player preferences; background send queue; install-banner polish.
