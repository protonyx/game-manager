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

### Full-Screen Game Experience (2026-05-02)

**What was built:**
- Added `hideChrome` input to `LayoutComponent` with `@HostBinding('class.hide-chrome')` to apply class to host element
- Layout template wraps toolbar with `@if (!hideChrome)` and changes sidenav `[opened]` to `[opened]="sidenavOpen && !hideChrome"`
- Layout styles updated: `.navbar-is-mobile:not(.hide-chrome) mat-sidenav-content` conditional margin-top prevents unwanted offset
- `app.component.ts` adds `isGameInProgress$` observable that maps `game?.state === 'InProgress'`
- `app.component.html` binds `[hideChrome]="(isGameInProgress$ | async) ?? false"` to layout (nullish coalescing required for strict null checks)
- `game-page.component.ts` imports `Router` and adds `onLeaveGame()` method that dispatches `GameActions.leaveGame()` + navigates to join page
- Between-turns header (`bt-header-content`) now includes entry code label and logout button after the timer
- My-turn header (`mt-header`) gets an overlay logout button positioned absolutely in top-right corner
- Compact player list completely redesigned: no outer `mat-card`, @if/@else branching renders horizontal card rows in compact mode
- Each compact player card: 6px color bar + 48px avatar circle + player name + badges (You/Playing) + tracker values
- Active player styling: border with player color + box-shadow glow
- Compact styles: dark theme, surface-container background, 12px border-radius, 16px padding, 12px margin-bottom

**Key patterns learned:**
- When hiding layout chrome for immersive views, entry code and navigation must be relocated INTO the feature component — not just hidden in the toolbar
- Use `@HostBinding('class.xyz')` to apply conditional classes to the component host element so child templates can reference them with `:not(.xyz)` selectors
- Nullish coalescing operator (`?? false`) is required when passing async boolean observables to strict boolean `@Input()` properties
- In Angular templates, `@if (condition) { ... } @else { ... }` branching is cleaner than `*ngIf` for structurally different rendering modes
- Compact/inline player cards should skip the `mat-card` wrapper entirely — use plain divs with custom styling to match dark-theme mockups
- Player badges ("You", "Playing") should be inline with the name row, not as subtitles, for horizontal card layouts
- Always position leave/logout buttons with `position: absolute` when overlaying on existing headers to avoid layout shift
- When adding Router navigation in components that already have Store injected, ensure Router is imported from `@angular/router` and injected in constructor

**Files modified:**
- `web/src/app/shared/layout/layout.component.ts` (hideChrome input + HostBinding)
- `web/src/app/shared/layout/layout.component.html` (conditional toolbar + sidenav)
- `web/src/app/shared/layout/layout.component.scss` (margin-top guard)
- `web/src/app/app.component.ts` (isGameInProgress$ observable)
- `web/src/app/app.component.html` (hideChrome binding)
- `web/src/app/game/pages/game-page/game-page.component.ts` (Router import + onLeaveGame method)
- `web/src/app/game/pages/game-page/game-page.component.html` (entry code + leave buttons in both headers)
- `web/src/app/game/pages/game-page/game-page.component.scss` (button styles for leave/entry-code)
- `web/src/app/game/components/player-list/player-list.component.html` (compact mode @if/@else branching)
- `web/src/app/game/components/player-list/player-list.component.scss` (compact-player-list styles)

### Tracker Card Margin Fix (2026-05-02)

**Issue:**
Tracker cards in the "My Turn" view lost their horizontal margins/padding after the layout refactor. Cards were rendering edge-to-edge with the screen edges.

**Root cause:**
`.mt-content` in `game-page.component.scss` had `padding: 8px 0` — only vertical padding, no horizontal. The `trackers-grid` had 16px gap between cards but no padding around the grid container itself.

**Fix applied:**
Changed `.mt-content` padding from `8px 0` to `8px 16px` to add 16px horizontal padding on both left and right sides.

**Files modified:**
- `web/src/app/game/pages/game-page/game-page.component.scss` (line 33: `.mt-content` padding updated)

**Key learnings:**
- When scrollable content containers have no horizontal padding, child grids render edge-to-edge
- Container-level padding is more reliable than per-item margins for consistent content inset
- Always verify mobile-first layouts visually after layout component changes — flex containers can lose padding when CSS specificity changes

### Host Controls in My Turn Footer (2026-05-02)

**What was built:**
- Added host controls button to the My Turn view footer (`.mt-footer`), sitting next to the End Turn button
- Button is a `mat-mini-fab` with primary color and settings/close icon toggle (reuses existing `fabOpen` state)
- Only visible when `vm.isHost` is true
- Opens an overlay menu with "End Game" action (styled as `mat-raised-button` with warn color)
- Menu uses fixed positioning at bottom-right, with backdrop scrim for modal dismissal
- Footer layout uses flexbox: End Turn button takes `flex: 1 1 auto`, host settings button is `flex: 0 0 auto` with 12px gap

**Key patterns learned:**
- The same `fabOpen` boolean can be reused across different layouts (My Turn and Between Turns) — no need for separate state variables
- Host controls in My Turn only need "End Game" action, not "Advance Turn" (host can use their own End Turn button)
- Fixed position overlays need explicit z-index layering: backdrop at z-index 100, menu at 101, footer at 10
- `mat-mini-fab` is better than `mat-fab` for secondary actions in a footer — keeps the primary action (End Turn) visually dominant
- Bottom-positioned overlays should account for `env(safe-area-inset-bottom)` in their positioning: `bottom: calc(68px + 12px + env(safe-area-inset-bottom))`
- Backdrop should cover the entire viewport (`position: fixed` with all edges at 0) and use semi-transparent black (`rgba(0, 0, 0, 0.5)`)

**Files modified:**
- `web/src/app/game/pages/game-page/game-page.component.html` (added host settings button and menu overlay in My Turn layout)
- `web/src/app/game/pages/game-page/game-page.component.scss` (footer flexbox, host button, menu and backdrop styles)

### My Turn Footer Pin Fix (2026-05-02)

**Issue:**
The "End Turn" button in the My Turn view was sliding down the page every time tracker increment/decrement buttons were clicked. The tracker editor component can toggle between normal mode and keypad mode, causing dramatic height changes in the tracker cards.

**Root cause:**
The `.mt-footer` was using `flex: 0 0 auto` within the flex layout, making it part of the normal document flow. When tracker cards expanded (especially when switching to keypad mode), the content area height changed, pushing the footer down — it wasn't truly pinned to the viewport bottom.

**Fix applied:**
- Changed `.mt-footer` from `flex: 0 0 auto` to `position: fixed` with `bottom: 0; left: 0; right: 0` to pin it to the viewport bottom
- Updated `.mt-content` padding-bottom from `8px 16px` to `calc(68px + 12px + env(safe-area-inset-bottom))` to provide clearance for the fixed footer
- Removed `padding-bottom: 96px` from `.trackers-grid` in `tracker-list.component.scss` — no longer needed since the footer is truly fixed

**Key learnings:**
- When footer buttons must remain accessible regardless of content changes, use `position: fixed` not `flex: 0 0 auto`
- Scrollable content areas need bottom padding equal to (footer height + gap + safe-area-inset) to prevent content from being obscured
- Child components shouldn't compensate for parent layout issues with hardcoded padding — fix the root layout instead
- Always test dynamic content height changes (like form mode toggles) to verify fixed positioning works correctly

**Files modified:**
- `web/src/app/game/pages/game-page/game-page.component.scss` (`.mt-footer` position fixed, `.mt-content` padding-bottom)
- `web/src/app/game/components/tracker-list/tracker-list.component.scss` (removed hardcoded padding-bottom)

