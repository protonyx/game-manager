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
