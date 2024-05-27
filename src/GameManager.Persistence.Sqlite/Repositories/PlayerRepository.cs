using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    public PlayerRepository(GameContext context)
        : base(context)
    {
    }

    public override async Task<Player?> GetByIdAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Player>().FindAsync(new object?[] { playerId }, cancellationToken);
    }

    public Task<int> GetActivePlayerCountAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return _context.Set<Player>()
            .Where(p => p.GameId == gameId && p.Active)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Player>> GetPlayersByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var players = await _context.Set<Player>()
            .AsQueryable()
            .AsNoTracking()
            .Include(t => t.TrackerValues)
            .Where(p => p.GameId == gameId && p.Active)
            .OrderBy(p => p.Order)
            .ToListAsync(cancellationToken);

        return players;
    }

    public async Task<bool> NameIsUniqueAsync(Guid gameId, PlayerName name, Guid? playerId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Player>()
            .AsQueryable()
            .Where(p => p.GameId == gameId && p.Active)
            .Where(p => ((string)(object)p.Name).ToLower() == name.Value.ToLower());

        if (playerId.HasValue)
        {
            query = query.Where(p => p.Id != playerId);
        }

        var playersWithName = await query.CountAsync(cancellationToken);

        return playersWithName == 0;
    }

    public async Task<bool> PlayerIsActiveAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Player>()
            .AsQueryable()
            .AnyAsync(t => t.Id == playerId && t.Active, cancellationToken: cancellationToken);
    }

    public async Task<bool> UpdateHeartbeatAsync(Guid playerId, string connectionId,
        CancellationToken cancellationToken = default)
    {
        var affectedResults = await _context.Set<PlayerConnection>()
            .AsQueryable()
            .Where(p => p.PlayerId == playerId && p.ConnectionId == connectionId)
            .ExecuteUpdateAsync(c =>
                    c.SetProperty(t => t.LastHeartbeat, DateTime.UtcNow),
                cancellationToken);

        return affectedResults > 0;
    }
}