using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    public PlayerRepository(GameContext context)
        : base(context)
    {
    }

    public override async Task<Player> CreateAsync(Player newPlayer)
    {
        var game = await _context.Set<Game>()
            .Include(t => t.Trackers)
            .FirstAsync(t => t.Id == newPlayer.GameId);

        var existingPlayers = await GetPlayersByGameIdAsync(newPlayer.GameId);

        //var totalPlayers = existingPlayers.Count();
        var maxOrder = existingPlayers.Any()
            ? existingPlayers.Max(t => t.Order)
            : 0;
        
        newPlayer.SetOrder(maxOrder + 1);

        if (!existingPlayers.Any(p => p.IsAdmin))
        {
            newPlayer.Promote();
        }

        newPlayer.TrackerValues = new List<TrackerValue>();

        foreach (var tracker in game.Trackers)
        {
            newPlayer.TrackerValues.Add(new TrackerValue()
            {
                Id = Guid.NewGuid(),
                PlayerId = newPlayer.Id,
                TrackerId = tracker.Id,
                Value = tracker.StartingValue
            });
        }

        _context.Add(newPlayer);
        
        await _context.SaveChangesAsync();

        return newPlayer;
    }

    public override async Task<Player?> GetByIdAsync(Guid playerId)
    {
        var player = await _context.Set<Player>()
            .AsNoTracking()
            .Include(t => t.TrackerValues)
            .FirstOrDefaultAsync(t => t.Id == playerId);

        return player;
    }

    public Task<int> GetActivePlayerCountAsync(Guid gameId)
    {
        return _context.Set<Player>()
            .Where(p => p.GameId == gameId && p.Active)
            .CountAsync();
    }

    public async Task<IReadOnlyList<Player>> GetPlayersByGameIdAsync(Guid gameId)
    {
        var players = await _context.Set<Player>()
            .AsQueryable()
            .AsNoTracking()
            .Include(t => t.TrackerValues)
            .Where(p => p.GameId == gameId && p.Active)
            .OrderBy(p => p.Order)
            .ToListAsync();

        return players;
    }
    
    public async Task<IReadOnlyList<Player>> GetSummariesByGameIdAsync(Guid gameId)
    {
        var players = await _context.Set<Player>()
            .AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Include(t => t.Turns)
            .Include(t => t.TrackerHistory)
            .Where(p => p.GameId == gameId)
            .OrderBy(p => p.Order)
            .ToListAsync();

        return players;
    }

    public override async Task<Player> UpdateAsync(Player updates)
    {
        var existing = await _context.Set<Player>()
            .Include(t => t.TrackerValues)
            .FirstOrDefaultAsync(t => t.Id == updates.Id);

        if (existing == null)
            throw new InvalidOperationException("Player not found");

        // Update name
        if (updates.Name != null)
        {
            existing.Name = updates.Name;
        }

        // Update trackers
        foreach (var tracker in existing.TrackerValues)
        {
            var trackerUpdate = updates.TrackerValues.FirstOrDefault(t => t.TrackerId == tracker.TrackerId);

            if (trackerUpdate != null)
            {
                _context.Set<TrackerHistory>().Add(new TrackerHistory()
                {
                    Id = Guid.NewGuid(),
                    PlayerId = existing.Id,
                    TrackerId = tracker.TrackerId,
                    ChangedTime = DateTime.UtcNow,
                    NewValue = trackerUpdate.Value
                });
                
                tracker.Value = trackerUpdate.Value;
            }
        }

        return await base.UpdateAsync(existing);
    }
    
    public async Task<bool> NameIsUniqueAsync(Guid gameId, string name, Guid? playerId = null)
    {
        var query = _context.Set<Player>()
            .AsQueryable()
            .Where(p => p.GameId == gameId && p.Active && p.Name.Equals(name));

        if (playerId.HasValue)
        {
            query = query.Where(p => p.Id != playerId);
        }
        
        var playersWithName = await query.CountAsync();

        return playersWithName == 0;
    }

    public Task UpdatePlayersAsync(ICollection<Player> players)
    {
        foreach (var player in players)
        {
            _context.Entry(player).State = EntityState.Modified;
        }
        
        return _context.SaveChangesAsync();
    }

    public async Task<ICollection<Player>> ReorderPlayersAsync(Guid gameId, IList<Guid> playerIds)
    {
        var players = await _context.Set<Player>()
            .Where(t => t.GameId == gameId && t.Active)
            .OrderBy(t => t.Order)
            .ToListAsync();
        
        // Reindex order starting at 1
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetOrder(i + 1);
        }

        await _context.SaveChangesAsync();
        
        return players;
    }

    public override async Task DeleteAsync(Player player)
    {
        player.SoftDelete();

        _context.Entry(player).State = EntityState.Modified;
            
        await _context.SaveChangesAsync();
            
        // Update player order for remaining players
        var players = await _context.Set<Player>()
            .Where(t => t.GameId == player.GameId && t.Active)
            .OrderBy(t => t.Order)
            .ToListAsync();
            
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetOrder(i + 1);
        }

        await _context.SaveChangesAsync();
    }
}