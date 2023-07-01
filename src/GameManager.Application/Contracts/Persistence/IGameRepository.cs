﻿using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface IGameRepository : IAsyncRepository<Game>
{
    Task<ICollection<Game>> FindAsync(DateTime? olderThan = null);
    
    Task<Game?> GetGameByEntryCodeAsync(string entryCode);

    Task<bool> EntryCodeExistsAsync(string entryCode);
}