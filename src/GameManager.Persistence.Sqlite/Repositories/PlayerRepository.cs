using System.ComponentModel.DataAnnotations;
using GameManager.Application.Data;
using GameManager.Application.Features.Games.Notifications;
using GameManager.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    private readonly IMediator _mediator;

    public PlayerRepository(GameContext context, IMediator mediator)
        : base(context)
    {
        _mediator = mediator;
    }

    public override async Task<Player> CreateAsync(Player newPlayer)
    {
        var game = await _context.Games
            .Include(t => t.Trackers)
            .FirstOrDefaultAsync(t => t.Id == newPlayer.GameId);

        if (game == null)
        {
            throw new ValidationException(new ValidationResult("Game does not exist",
                new[] {nameof(Player.GameId)}), null, newPlayer.GameId);
        }

        var existingPlayers = await GetPlayersByGameId(newPlayer.GameId);

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
        newPlayer.Active = true;
        newPlayer.Order = maxOrder + 1;
        newPlayer.LastHeartbeat = DateTime.UtcNow;

        if (!existingPlayers.Any(p => p.IsAdmin))
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

        await _mediator.Publish(new PlayerCreatedNotification(newPlayer));

        // Update game state if no current turn
        if (!game.CurrentTurnPlayerId.HasValue)
        {
            var startPlayer = existingPlayers.MinBy(t => t.Order);
            game.CurrentTurnPlayerId ??= startPlayer?.Id;
            
            await _context.SaveChangesAsync();

            await _mediator.Publish(new GameUpdatedNotification(game));
        }

        return newPlayer;
    }

    public override async Task<Player?> GetByIdAsync(Guid playerId)
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
            .Where(p => p.GameId == gameId && p.Active)
            .OrderBy(p => p.Order)
            .ToListAsync();

        return players;
    }

    public override async Task<Player?> UpdateAsync(Player updates)
    {
        var existing = await _context.Players
            .Include(t => t.TrackerValues)
            .FirstOrDefaultAsync(t => t.Id == updates.Id);

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

        await base.UpdateAsync(existing);

        await _mediator.Publish(new PlayerUpdatedNotification(existing));
        
        // Update player orders, if changed
        if (existing.Order != updates.Order)
        {
            var players = await _context.Players
                .Where(t => t.GameId == existing.GameId && t.Active)
                .OrderBy(t => t.Order)
                .ToListAsync();

            var fromIndex = players.FindIndex(p => p.Id == updates.Id);
            var toIndex = updates.Order - 1;

            var target = players[fromIndex];
            var delta = toIndex < fromIndex ? -1 : 1;

            for (int i = fromIndex; i != toIndex; i += delta)
            {
                players[i] = players[i + delta];
            }

            players[toIndex] = target;

            // Reindex order starting at 1
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Order = i + 1;
            }

            await _context.SaveChangesAsync();

            foreach (var player in players)
            {
                await _mediator.Publish(new PlayerUpdatedNotification(player));
            }
        }

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

    public override async Task DeleteAsync(Player player)
    {
        player.Active = false;
        player.Order = 0;
            
        await _context.SaveChangesAsync();
            
        // Update player order for remaining players
        var players = await _context.Players
            .Where(t => t.GameId == player.GameId && t.Active)
            .OrderBy(t => t.Order)
            .ToListAsync();
            
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Order = i + 1;
        }

        await _context.SaveChangesAsync();

        await this._mediator.Publish(new PlayerDeletedNotification(player));
    }
}