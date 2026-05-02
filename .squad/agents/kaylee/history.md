# Kaylee — History

## Core Context

Frontend Dev for game-manager. Stack: Angular 20 / NgRx / Angular Material / TypeScript. Mobile-first. Frontend root: `web/`. Game feature at `web/src/app/game/`. Real-time via `@microsoft/signalr`. Dev: `cd web && npm start`. Tests: `cd web && npm test`.

User: Protonyx (Kevin)

## Learnings

### Lobby Experience Implementation (2025-04)

**What was built:**
- Added `isReady?: boolean` to the `Player` model/interface
- Added `GameActions.setPlayerReady` NgRx action + functional effect that maps to `GamesApiActions.patchPlayer` with `[{ op: 'replace', path: '/isReady', value: isReady }]`
- Created `app-lobby-player-card` — displays avatar (color circle + initial), name, and ready status
- Created `app-host-lobby` — responsive player grid (2 col mobile → 4 col desktop), Start Game (disabled until all non-host ready), Start Anyways (window.confirm guard)
- Created `app-player-waiting` — game info header, inline-editable name (emits patch on blur), 12-swatch color picker (same pattern as player-edit), big ready toggle button
- Updated `game-page.component.ts/.html` to conditionally render lobby views when `game.state === 'Preparing'`; moved `players$`, `isHost$`, `takenColors$` into the ngrxLet VM

**Key patterns learned:**
- All components are standalone (no NgModule). No `--standalone=false` needed.
- Effects are functional: `createEffect(() => ..., { functional: true })`
- `ngOnChanges` is NOT auto-called in Karma tests when setting `component.input = value` directly. Must call `component.ngOnChanges()` explicitly before `fixture.detectChanges()`.
- `GamesApiActions.patchPlayer` doubles as a "request" action (dispatched by effects to trigger HTTP) even though it's in the API actions group
- SignalR hub push (`GameHubActions.playerUpdated`) is what actually updates the store after a PATCH — no need to handle the HTTP success in the reducer
- Color picker: the `player-edit` component pattern (circular swatches, `.selected`/`.taken` classes, `check` icon overlay) should be borrowed across the app
- `<label>` for non-form controls triggers `@angular-eslint/template/label-has-associated-control`; use `<p>` instead
- The `prefer-inject` constructor lint errors and `exhaust` unused import in `game.effects.ts` are pre-existing issues throughout the codebase

### Bug Fixes (2025-04)

**Bugs fixed:**
- **Host status in lobby card**: `LobbyPlayerCardComponent` got a new `@Input() isHost = false`. When `isHost` is true the card shows a "Host" label (styled in primary color, uppercase) and suppresses the ready/waiting labels and ready badge. `HostLobbyComponent` passes `[isHost]="player.id === currentPlayer?.id"` to each card.
- **Color picker removed from join form**: `JoinGameComponent` had a 12-swatch color picker that was removed. The `color` form control (previously `Validators.required`) was dropped entirely. Form is now valid with just `entryCode` + `playerName`. `onSubmit()` no longer sends a color. `MatIconModule`, `MatTooltipModule`, and `PLAYER_COLORS` imports removed from the join component. Color selection lives only in `app-player-waiting`.
- **Host edit controls in lobby**: `HostLobbyComponent` gained `@Output() editPlayer = new EventEmitter<Player>()` and `onEditPlayer(player)`. Each player card in the grid is now wrapped in `.player-card-wrapper` containing an `edit` icon button. `game-page.component.html` wires `(editPlayer)="onPlayerEdit($event)"` on `app-host-lobby`, routing through the existing `GameActions.editPlayer` dispatch that opens the player-edit dialog.

**Patterns confirmed:**
- `@typescript-eslint/no-inferrable-types`: don't annotate boolean/string/number inputs with their obvious type (e.g. `@Input() isHost = false` not `@Input() isHost: boolean = false`)
- Pre-existing test failures (AuthInterceptorService, TrackerEditorDialogComponent) are unrelated to lobby work — baseline 4 failures, 158 successes

### Observer Waiting View (2025-04)

**What was built:**
- Added `selectCurrentPlayerIsObserver` selector: detects observer by checking `credentials != null && currentPlayer == null`. When a user joins as observer, the backend does NOT create a player entity (Guid.Empty playerId), so the player store has no matching entity.
- Created `app-observer-waiting` standalone component: `mat-spinner` + "Waiting for the game to start..." message, no interactive elements.
- Updated `game-page.component.html` lobby routing to three branches:
  - Host → `app-host-lobby`
  - Player (non-host, non-observer) → `app-player-waiting` (`*ngIf="!vm.isHost && !vm.isObserver"`)
  - Observer → `app-observer-waiting` (`*ngIf="vm.isObserver"`)

**Key facts learned:**
- Observers join via `observer: true` in JoinGameCommand. The backend skips player creation and returns credentials with `PlayerId = Guid.Empty`. No player entity ever appears in the store for an observer.
- Detecting observer: `credentials` set but `selectCurrentPlayer` is `undefined` — simplest reliable signal.
- All components in this codebase are standalone (use `imports: []` in `@Component`) — task instructions saying "standalone: false + NgModule" were incorrect for this repo.

### Between Turns — Host Controls Placement Mockup (2025)

**What was built:**
- Created `host-controls-mockup.html` in session files: side-by-side phone-frame (375×667px) comparison of two host-controls layouts
- **Option A**: taller footer (~110px) holds "Up Next" info row + two action buttons (Advance Turn in green, End Game as red outline)
- **Option B**: minimal 64px footer ("Up Next" only) + a ⚙ FAB anchored bottom-right; tap reveals an animated pop-up menu with Advance Turn + End Game rows; tap scrim or FAB again to dismiss
- Both frames share: pinned 72px header (avatar circle, player name, live ticking timer), scrollable middle zone with 4 compact player cards, dark theme (`--bg-primary: #1a1a1a`)
- FAB uses CSS `opacity + transform` transition (no JS jank), scrim overlay, and `rotate(45deg)` on the trigger icon to signal open state

**Design decisions made:**
- Option A footer height: ~110px (Up Next row ~32px + gap + action row ~46px + padding); this meaningfully reduces scroll real estate on 375px width
- Option B FAB sits at `bottom: 80px` (above the 64px footer + 16px gap), right-aligned; actions stagger upward with label chips for legibility
- Chose ▶ / ✕ icons (Unicode) to avoid external icon dependencies
- Added a scrim backdrop in Option B so the FAB feels modal and dismisses easily

**Key facts:**
- Mockup is fully self-contained (Google Fonts import only, no CDN JS/CSS)
- Both timers tick in sync from a single `setInterval` — same elapsed time shown on both frames
