# Jayne — History

## Core Context

Tester for game-manager. Backend tests: `dotnet test src/GameManager.Tests/GameManager.Tests.csproj`. Frontend tests: `cd web && npm test`. Key test areas: game creation, player join, turn advancement, score tracking, SignalR disconnects, entry code validation.

User: Protonyx (Kevin)

## Learnings

### 2026-04-20 — Lobby Redesign Review (commit `b9eccbc`)

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
