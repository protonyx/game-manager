using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Contracts.Persistence;

public interface IGameRepository : IAsyncRepository<Game>
{
    IQueryable<Game> Query();

    Task<IReadOnlyList<Game>> FindAsync(DateTime? olderThan = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> GetIdByEntryCodeAsync(EntryCode entryCode, CancellationToken cancellationToken = default);

    Task<bool> EntryCodeExistsAsync(EntryCode entryCode, CancellationToken cancellationToken = default);
}