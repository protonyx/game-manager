using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<ICollection<Player>> GetPlayersByGameIdAsync(Guid gameId);
    Task<bool> NameIsUniqueAsync(Guid gameId, string name, Guid? playerId = null);
    Task UpdatePlayerHeartbeatAsync(Guid playerId);
    Task UpdatePlayersAsync(IEnumerable<Player> players);
}