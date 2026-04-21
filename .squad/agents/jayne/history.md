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
