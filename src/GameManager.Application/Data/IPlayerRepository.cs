using GameManager.Domain.Entities;

namespace GameManager.Application.Data;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<ICollection<Player>> GetPlayersByGameId(Guid gameId);
    Task<bool> NameIsUnique(Guid gameId, string name);
    Task UpdatePlayerHeartbeat(Guid playerId);
}