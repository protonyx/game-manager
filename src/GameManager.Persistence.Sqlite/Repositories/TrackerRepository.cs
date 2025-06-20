using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class TrackerRepository : BaseRepository<Tracker>, ITrackerRepository
{
    public TrackerRepository(GameContext context)
        : base(context)
    {
    }
}