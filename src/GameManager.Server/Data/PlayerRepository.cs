using System.ComponentModel.DataAnnotations;
using GameManager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Data;

public class PlayerRepository
{
    private readonly GameContext _context;

    public PlayerRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Player> CreatePlayerAsync(Guid gameId, Player newPlayer)
    {
        var game = await _context.Games
            .Include(t => t.Trackers)
            .FirstOrDefaultAsync(t => t.Id == gameId);

        var existingPlayers = await GetPlayersByGameId(gameId);

        if (existingPlayers.Any(p => p.Name.Equals(newPlayer.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException(new ValidationResult("Player already exists with that name",
                new[] {nameof(Player.Name)}), null, newPlayer.Name);
        }
        
        var totalPlayers = existingPlayers.Count();
        var maxOrder = existingPlayers.Any()
            ? existingPlayers.Max(t => t.Order)
            : 0;

        newPlayer.Id = Guid.NewGuid();
        newPlayer.GameId = gameId;
        newPlayer.Active = true;
        newPlayer.Order = maxOrder + 1;
        newPlayer.LastHeartbeat = DateTime.UtcNow;

        if (totalPlayers == 0)
        {
            newPlayer.IsAdmin = true;
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

        if (!game.CurrentTurnPlayerId.HasValue)
        {
            var startPlayer = existingPlayers.MinBy(t => t.Order);
            game.CurrentTurnPlayerId ??= startPlayer?.Id;
        }

        await _context.SaveChangesAsync();

        return newPlayer;
    }

    public async Task<Player?> GetPlayerById(Guid playerId)
    {
        var player = await _context.Players
            .AsNoTracking()
            .Include(t => t.TrackerValues)
            .FirstOrDefaultAsync(t => t.Id == playerId);

        return player;
    }

    public async Task<ICollection<Player>> GetPlayersByGameId(Guid gameId)
    {
        var players = await _context.Players
            .AsQueryable()
            .AsNoTracking()
            .Include(t => t.TrackerValues)
            .Where(p => p.GameId == gameId)
            .OrderBy(p => p.Order)
            .ToListAsync();

        return players;
    }

    public async Task<Player?> UpdatePlayerAsync(Guid playerId, Player updates)
    {
        var existing = await _context.Players
            .Include(t => t.TrackerValues)
            .FirstOrDefaultAsync(t => t.Id == playerId);

        if (existing == null)
            return null;

        if (!string.IsNullOrWhiteSpace(updates.Name))
        {
            existing.Name = updates.Name;
        }

        // Update trackers
        foreach (var tracker in existing.TrackerValues)
        {
            var trackerUpdate = updates.TrackerValues.FirstOrDefault(t => t.TrackerId == tracker.TrackerId);

            if (trackerUpdate != null)
            {
                tracker.Value = trackerUpdate.Value;
            }
        }

        await _context.SaveChangesAsync();
        
        // Update player orders, if changed
        if (existing.Order != updates.Order)
        {
            var players = await _context.Players
                .Where(t => t.GameId == existing.GameId)
                .ToListAsync();

            // Move all players down
            foreach (var player in players.Where(p => p.Order >= updates.Order))
            {
                player.Order += 1;
            }
            
            existing.Order = updates.Order;
            
            players.Sort(new PlayerComparer());

            // Reindex order starting at 1
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Order = i + 1;
            }
            
            // TODO: Send notifications for all updated players

            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();

        return existing;
    }

    internal class PlayerComparer : IComparer<Player>
    {
        public int Compare(Player x, Player y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Order.CompareTo(y.Order);
        }
    }

    public async Task UpdatePlayerHeartbeat(Guid playerId)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(t => t.Id == playerId);

        if (player == null)
        {
            return;
        }

        player.LastHeartbeat = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RemovePlayer(Guid playerId)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(t => t.Id == playerId);

        if (player != null)
        {
            _context.Players.Remove(player);

            await _context.SaveChangesAsync();
        }
    }
}