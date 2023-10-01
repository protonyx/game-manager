using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(GameContext context)
        : base(context)
    {
    }
    
    public async Task<IReadOnlyList<Game>> FindAsync(DateTime? olderThan = null)
    {
        IQueryable<Game> query = _context.Set<Game>()
            .AsQueryable()
            .AsNoTracking();

        if (olderThan.HasValue)
        {
            query = query.Where(t => t.CreatedDate < olderThan);
        }

        var games = await query.ToListAsync();

        return games;
    }

    public override async Task<Game> CreateAsync(Game game)
    {
        game.Id = Guid.NewGuid();
        game.CreatedDate = DateTime.UtcNow;

        return await base.CreateAsync(game);
    }

    public override Task<Game> UpdateAsync(Game entity)
    {
        if (entity.CurrentTurn != null)
        {
            var entry = _context.Entry(entity.CurrentTurn);

            if (entry.State == EntityState.Detached)
            {
                _context.Attach(entity.CurrentTurn);
            }
        }
        
        return base.UpdateAsync(entity);
    }

    public override async Task<Game?> GetByIdAsync(Guid gameId)
    {
        IQueryable<Game> queryable = _context.Set<Game>()
            .AsQueryable()
            .AsNoTracking()
            .Include(t => t.Options)
            .Include(t => t.Trackers);

        var game = await queryable.Where(t => t.Id == gameId)
            .FirstOrDefaultAsync();

        return game;
    }

    public async Task<Game?> GetGameByEntryCodeAsync(EntryCode entryCode)
    {
        var game = await _context.Set<Game>()
            .Where(t => t.EntryCode == entryCode)
            .FirstOrDefaultAsync();

        return game;
    }

    public Task<bool> EntryCodeExistsAsync(string entryCode)
    {
        return _context.Set<Game>()
            .AnyAsync(t => t.EntryCode.Equals(entryCode));
    }
}