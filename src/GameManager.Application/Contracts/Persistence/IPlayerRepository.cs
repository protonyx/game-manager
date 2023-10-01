namespace GameManager.Application.Contracts.Persistence;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<int> GetActivePlayerCountAsync(Guid gameId);
    Task<IReadOnlyList<Player>> GetPlayersByGameIdAsync(Guid gameId);
    Task<IReadOnlyList<Player>> GetSummariesByGameIdAsync(Guid gameId);
    Task<bool> NameIsUniqueAsync(Guid gameId, string name, Guid? playerId = null);
    Task UpdatePlayersAsync(ICollection<Player> players);
}