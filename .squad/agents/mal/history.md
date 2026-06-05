# Mal — History

## Core Context

Lead for game-manager. Stack: .NET 10 / ASP.NET Core / EF Core / SQLite / SignalR / Redis / Aspire (backend) · Angular 20 / NgRx / Angular Material (frontend). Mobile-first turn-based game manager — players join via entry code, get SignalR notifications on their turn.

User: Protonyx (Kevin)

## Learnings

### 2025-01-20: Lobby Redesign Architecture

**Context analyzed:**
- Current game-page renders all states (Preparing/InProgress/Complete) in one view
- Player entity has no `IsReady` field — needs to be added
- `PlayerUpdated` SignalR notification already broadcasts full PlayerDTO — adding IsReady will propagate automatically
- Existing `PATCH /api/v1/players/{id}` can handle ready state via JSON Patch
- Player colors are chosen at join time (12 colors available)
- Host controls exist in `game-control` component (start/end buttons)

**Design decision:** Views split by role during Preparing state:
- Non-host players → player-waiting view (mobile-first, name/color edit, ready toggle)
- Host → host-lobby view (player cards with ready indicators, start controls)

**Key insight:** Keep as views within game-page, not separate routes. Preserves auth/state management.

**Work assigned:** Zoe (backend: IsReady field + DTO updates) | Kaylee (frontend: new components + game-page conditional rendering)

### 2026-06-04: Code Review — Mapperly AutoMapper Migration

Comprehensive review of feature/replace-automapper branch completed.

**Findings & Approvals:**
1. ✅ Approved overall Mapperly migration approach — sound architecture, good use of `RequiredMappingStrategy`
2. ✅ Approved wrapper patterns for custom mapping logic (e.g., `PlayerToDto` with state calculation)
3. ✅ Approved singleton registration of `DtoMapper` in DI container
4. ⚠️ Identified **DtoToGameOptions null crash**: non-nullable signature on Mapperly partial method, but nullable DTOs passed at call site → **Fixed by Zoe**
5. ⚠️ Noted IQueryable projection concern in PlayerToDto — monitor for performance issues

**Team Actions:**
- Zoe: Fixed nullable crash signature `GameOptions? DtoToGameOptions(GameOptionsDTO? dto)` — builds clean, all 51 tests pass
- Jayne: Added 6 new tests for PlayerToDto state logic and TrackerValues mapping — fills critical gap, all 51 tests pass

**Decisions Logged:**
1. Use nullable signatures for optional Mapperly mappings (pattern: nullable input/output, call site handles default with `??`)
2. Dedicated test files for mapper business logic (pattern: separate DtoMapperTests.cs from query handler tests)

**Status**: All issues resolved. Ready for merge.

