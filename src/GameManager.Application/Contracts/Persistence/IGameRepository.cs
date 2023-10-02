using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Contracts.Persistence;

public interface IGameRepository : IAsyncRepository<Game>
{
    Task<IReadOnlyList<Game>> FindAsync(DateTime? olderThan = null, CancellationToken cancellationToken = default);
    
    Task<Game?> GetGameByEntryCodeAsync(EntryCode entryCode, CancellationToken cancellationToken = default);

    Task<bool> EntryCodeExistsAsync(EntryCode entryCode, CancellationToken cancellationToken = default);
}