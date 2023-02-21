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
            .AsNoTracking()
            .Include(t => t.Trackers)
            .FirstOrDefaultAsync(t => t.Id == gameId);

        var existingPlayers = await GetPlayersByGameId(gameId);

        if (existingPlayers.Any(p => p.Name.Equals(newPlayer.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException(new ValidationResult("Player already exists with that name",
                new[] {nameof(Player.Name)}), null, newPlayer.Name);
        }
        
        var totalPlayers = existingPlayers.Count();

        newPlayer.Id = Guid.NewGuid();
        newPlayer.GameId = gameId;
        newPlayer.Token = string.Empty; //Guid.NewGuid().ToString();
        newPlayer.Order = totalPlayers + 1;

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
            .Where(p => p.GameId == gameId)
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
                .Where(t => t.GameId == existing.GameId && t.Order > updates.Order)
                .ToListAsync();

            foreach (var player in players)
            {
                player.Order += 1;
            }

            await _context.SaveChangesAsync();
        }

        existing.Order = updates.Order;

        await _context.SaveChangesAsync();

        return existing;
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