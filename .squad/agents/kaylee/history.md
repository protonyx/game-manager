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
