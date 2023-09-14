using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Contracts.Persistence;

public interface IGameRepository : IAsyncRepository<Game>
{
    Task<ICollection<Game>> FindAsync(DateTime? olderThan = null);
    
    Task<Game?> GetGameByEntryCodeAsync(EntryCode entryCode);

    Task<bool> EntryCodeExistsAsync(string entryCode);
}