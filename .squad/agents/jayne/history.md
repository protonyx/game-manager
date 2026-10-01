# Jayne — History

## Core Context

Tester for game-manager. Backend tests: `dotnet test src/GameManager.Tests/GameManager.Tests.csproj`. Frontend tests: `cd web && npm test`. Key test areas: game creation, player join, turn advancement, score tracking, SignalR disconnects, entry code validation.

User: Protonyx (Kevin)

## Learnings

### 2026-04-21 — ColorPickerComponent Tests (`color-picker.component.spec.ts`)

Wrote 22 comprehensive tests for the new standalone `ColorPickerComponent`.

**Test coverage:** creation, all PLAYER_COLORS rendered, label input (default + update), `.selected` class, `.taken` class + disabled, click-to-emit, taken click no-emit, case-insensitive matching (both isSelected and isTaken), check/taken icons, empty selectedColor, empty takenColors, `select()` method directly.

**Patterns used:**
- Standalone component: `imports: [ColorPickerComponent, NoopAnimationsModule]` (NOT `declarations`)
- Used `By.css('.color-swatch')` queries via `fixture.debugElement.queryAll()`
- Clicked taken swatch via `component.select(hex)` directly (disabled buttons don't fire DOM clicks)
- Used `spyOn(component.colorSelected, 'emit')` for emission verification
- `fixture.detectChanges()` called after mutating `@Input()` properties directly

**Baseline note:** `git stash` does NOT stash untracked files — so a new (untracked) spec file will run in both baseline and post-change runs. The true baseline diff is confirmed with `fdescribe` (all 22 focus-run tests passed).

**Adjacent fixes:** The ColorPickerComponent refactoring left 3 stale spec files with broken tests:
- `player-edit.component.spec.ts`: removed `isColorSelected`/`isColorTaken` tests (moved to child component); fixed `selectColor() does nothing when taken` (taken guard is now in ColorPickerComponent, not PlayerEditComponent)
- `player-waiting.component.spec.ts`: same — replaced `color swatch states` block with correct behavior test
- `host-lobby.component.spec.ts`: edit button removed, replaced with clickable `.player-card-wrapper` div; updated test to click the wrapper instead

**Pre-existing failures (4):** `TrackerEditorDialogComponent should create` (missing Store provider), `AuthInterceptorService` ×3 (uses `inject` before configureTestingModule). These are unchanged from baseline.


Reviewed full `IsReady` backend + lobby frontend feature. **PASS WITH NOTES.**

**Pattern observed:** Pre-existing lint errors (`exhaust` unused import, `TrackerEditorComponent` unused import) and 4 pre-existing failing frontend tests. Always baseline before accusing a feature of introducing failures — `git stash` + run tests confirms pre-existing state.

**Bugs found:**
1. `PlayerModel.cs` (GraphQL) missing `IsReady` — REST API correct, GraphQL layer inconsistent. Zoe missed this.
2. No test for unauthorized cross-player `isReady` patch — authorization logic is correct but untested for this path.

**Architecture observations:**
- `isHost` is correctly sourced from `PlayerCredentials` (JWT claims) not from the player entity on the frontend. `selectCurrentPlayerIsHost` uses `credentials.isHost`. This is the right pattern.
- The PATCH endpoint pattern: load PlayerDTO → map to UpdatePlayerDTO → apply JSON Patch → save. `DefaultContractResolver` used but camelCase paths work (consistent with pre-existing color/name patches).
- `playerAdapter.setOne` in the reducer correctly handles SignalR `PlayerUpdated` messages — whole entity replaced, so `isReady` propagates correctly.
- `allReady` correctly guards with `=== true` (not just truthy) to handle the `isReady?: boolean` optional case where undefined would incorrectly pass a truthiness check.

### 2026-06-04 — PlayerToDto Mapper Test Coverage (`DtoMapperTests.cs`)

Added 6 comprehensive tests for `PlayerToDto` mapper business logic that was previously untested.

**Test coverage:**
1. `State = Disconnected` when `Connections.Count == 0`
2. `State = Connected` when `Connections.Count > 0` (single connection)
3. `State = Connected` with multiple connections (verified 3 connections)
4. `TrackerValues` dictionary correctly maps `TrackerId → Value` (tested 3 trackers with different values)
5. Empty `TrackerValues` returns empty dictionary (not null)
6. All properties map correctly end-to-end (Id, Name, IsHost, IsReady, Color, State)

**Patterns used:**
- Created new dedicated test file `src/GameManager.Tests/Mappers/DtoMapperTests.cs` for mapper-specific tests (cleaner separation from query handler tests)
- Used real `DtoMapper` instance (not mocked) — consistent with existing `GetPlayerQueryTests` pattern
- Used reflection to set private fields (`_trackers` list, `Id` property) when needed for test setup
- Used `Tracker.Create()` factory method for proper Tracker entity construction
- Used `Player.AddConnection()` public API to test connection state logic
- FluentAssertions for all assertions (consistent with existing test style)

**Domain entity construction pattern:**
- `Game` + `GameOptions` → `new Game(GameName, GameOptions)`
- `Tracker.Create(game, TrackerName, startingValue)` returns `Result<Tracker>`
- `Player` constructor auto-initializes `TrackerValues` from game's trackers with starting values
- `player.SetTracker(trackerId, value)` modifies tracker values (returns `Result`)
- `player.AddConnection(connectionId)` adds to internal `_connections` list

**Test execution:**
- All 51 tests passed (45 existing + 6 new)
- Run command: `dotnet run --project src/GameManager.Tests/GameManager.Tests.csproj --no-build`
- Note: `dotnet test` command hung indefinitely on this environment; `dotnet run` on the test project worked correctly with xUnit v3 Microsoft.Testing.Platform

**Business logic validated:**
- The `PlayerToDto` wrapper correctly applies connection-based state logic (core business rule for player connectivity)
- TrackerValues dictionary mapping preserves TrackerId keys and current values (critical for game state serialization)
- Empty collections handled correctly (no null reference issues)
