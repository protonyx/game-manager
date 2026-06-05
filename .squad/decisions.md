# Squad Decisions

## Active Decisions

### 1. Use Nullable Signatures for Optional Mapperly Mappings

**Status**: Approved  
**Date**: 2026-06-04  
**Author**: Zoe (Backend Dev)  

Mapperly partial method signatures should match the nullability semantics of the data flow:
- When a DTO can be null at the call site, use nullable input: `GameOptionsDTO?`
- When a null input should produce a null output, use nullable output: `GameOptions?`
- Let the call site handle the null-to-default fallback (e.g., `?? new GameOptions()`)

**Rationale**: Type safety, defensive code generation, clear responsibility separation, consistency with C# nullable reference types.

**Example**:
```csharp
public partial GameOptions? DtoToGameOptions(GameOptionsDTO? dto);
var options = _mapper.DtoToGameOptions(request.Options) ?? new GameOptions();
```

**Consequences**: Runtime null crashes prevented; intent explicit. Requires nullable annotations on partial methods (standard practice).

---

### 2. Dedicated Test Files for Mapper Business Logic

**Status**: Approved  
**Date**: 2026-06-04  
**Author**: Jayne (Tester)  

Custom business logic in mapper wrapper methods (e.g., `PlayerToDto`) requires dedicated test coverage separate from query handler tests.

**Rationale**: Query handler tests focus on query execution and infrastructure. Mapper tests focus on pure transformation logic and business rules. Cleaner separation of concerns.

**Pattern**: Created `src/GameManager.Tests/Mappers/DtoMapperTests.cs` for mapper-specific tests.

**Coverage Added**:
- PlayerToDto state logic (disconnected/connected)
- TrackerValues dictionary mapping
- Edge case handling (empty collections)

**Implications**: When adding custom logic to mapper wrappers, add corresponding tests to `DtoMapperTests.cs`. Reviewers should flag untested mapper wrapper methods.

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
