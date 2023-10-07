using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface ITurnRepository : IAsyncRepository<Turn>
{
    Task<IReadOnlyList<Turn>> GetTurnsByPlayerId(Guid playerId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Turn>> GetTurnsByGameId(Guid gameId, CancellationToken cancellationToken = default);
}