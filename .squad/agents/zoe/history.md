# Zoe — History

## Core Context

Backend Dev for game-manager. Stack: .NET 10 / ASP.NET Core / EF Core / SQLite / SignalR / Redis / Aspire. Clean architecture: Domain → Application → Persistence.Sqlite → Server. Solution at `src/GameManager.sln`. Tests at `src/GameManager.Tests/`.

User: Protonyx (Kevin)

## Learnings

### AddPlayerIsReady (2025)
Added `IsReady` (bool) to the `Player` domain entity and wired it through the full stack:
- **Domain**: Added `IsReady { get; private set; }`, `SetReady()`, `ClearReady()` methods; updated copy constructor.
- **DTOs**: Added `IsReady` to `PlayerDTO` (bool); added `IsReady?` to `UpdatePlayerDTO` (nullable bool for patch semantics).
- **AutoMapper**: No explicit mapping needed — `Player → PlayerDTO` and `PlayerDTO → UpdatePlayerDTO` both auto-map by convention.
- **Command handler**: Added null-check pattern `if (request.Player.IsReady.HasValue)` → `SetReady()` / `ClearReady()`, consistent with how `Color` and `Name` are patched.
- **EF Config**: Added `builder.Property(t => t.IsReady).IsRequired().HasDefaultValue(false)` to `PlayerConfiguration`.
- **Migration**: Created `AddPlayerIsReady` migration via `dotnet ef migrations add AddPlayerIsReady --startup-project ../GameManager.Server` from `GameManager.Persistence.Sqlite/`.
- **Tests**: Added `PlayerTests` domain unit tests (4 tests); added 3 new `UpdatePlayerCommandTests` for `IsReady = true/false/null` paths. All 45 tests pass.
- The PATCH endpoint uses AutoMapper `PlayerDTO → UpdatePlayerDTO` then JSON Patch, so `IsReady?` in `UpdatePlayerDTO` naturally flows through without endpoint changes.
