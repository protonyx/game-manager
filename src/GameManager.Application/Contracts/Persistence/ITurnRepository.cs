using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface ITurnRepository : IAsyncRepository<Turn>
{
    Task<IReadOnlyList<Turn>> GetTurnsByPlayerId(Guid playerId);
}