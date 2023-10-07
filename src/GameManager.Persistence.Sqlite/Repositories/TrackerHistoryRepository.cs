using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class TrackerHistoryRepository : BaseRepository<TrackerHistory>, ITrackerHistoryRepository
{
    public TrackerHistoryRepository(GameContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<TrackerHistory>> GetHistoryByGameId(Guid gameId, CancellationToken cancellationToken = default)
    {
        var history = await _context.Set<TrackerHistory>()
            .AsQueryable()
            .AsNoTracking()
            .Where(t => t.Player.GameId == gameId)
            .ToListAsync(cancellationToken);

        return history;
    }
}