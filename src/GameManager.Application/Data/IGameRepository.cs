﻿using GameManager.Domain.Entities;

namespace GameManager.Application.Data;

public interface IGameRepository : IAsyncRepository<Game>
{
    Task<Game?> GetGameByEntryCodeAsync(string entryCode);
}