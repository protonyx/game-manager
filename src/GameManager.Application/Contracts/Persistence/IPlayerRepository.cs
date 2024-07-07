using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Contracts.Persistence;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<int> GetActivePlayerCountAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetIdsByGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<bool> NameIsUniqueAsync(Guid gameId, PlayerName name, Guid? playerId = null, CancellationToken cancellationToken = default);
    Task<bool> PlayerIsActiveAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<bool> UpdateHeartbeatAsync(Guid playerId, string connectionId, CancellationToken cancellationToken = default);
}