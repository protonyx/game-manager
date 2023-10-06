namespace GameManager.Application.Contracts.Persistence;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<int> GetActivePlayerCountAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetPlayersByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlayerSummary>> GetSummariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<bool> NameIsUniqueAsync(Guid gameId, string name, Guid? playerId = null, CancellationToken cancellationToken = default);
}