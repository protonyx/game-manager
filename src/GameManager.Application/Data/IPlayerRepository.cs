using GameManager.Domain.Entities;

namespace GameManager.Application.Data;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<ICollection<Player>> GetPlayersByGameIdAsync(Guid gameId);
    Task<bool> NameIsUniqueAsync(Guid gameId, string name, Guid? playerId = null);
    Task UpdatePlayerHeartbeatAsync(Guid playerId);
}