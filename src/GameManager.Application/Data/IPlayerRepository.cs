﻿using GameManager.Domain.Entities;

namespace GameManager.Application.Data;

public interface IPlayerRepository : IAsyncRepository<Player>
{
    Task<ICollection<Player>> GetPlayersByGameId(Guid gameId);
    Task UpdatePlayerHeartbeat(Guid playerId);
}