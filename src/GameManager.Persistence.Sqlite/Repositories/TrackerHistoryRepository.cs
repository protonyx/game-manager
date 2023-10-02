using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;

namespace GameManager.Persistence.Sqlite.Repositories;

public class TrackerHistoryRepository : BaseRepository<TrackerHistory>, ITrackerHistoryRepository
{
    public TrackerHistoryRepository(GameContext context)
        : base(context)
    {
    }
}