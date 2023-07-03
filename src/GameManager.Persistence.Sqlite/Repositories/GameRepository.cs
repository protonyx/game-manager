using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    private readonly IMediator _mediator;

    public GameRepository(GameContext context, IMediator mediator)
        : base(context)
    {
        _mediator = mediator;
    }
    
    public async Task<ICollection<Game>> FindAsync(DateTime? olderThan = null)
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

    public override async Task UpdateAsync(Game entity)
    {
        await base.UpdateAsync(entity);
        
        await _mediator.Publish(new GameUpdatedNotification(entity));
    }

    public async Task<Game?> GetGameByEntryCodeAsync(string entryCode)
    {
        var game = await _context.Set<Game>()
            .Where(t => t.EntryCode == entryCode.ToUpper())
            .FirstOrDefaultAsync();

        return game;
    }

    public Task<bool> EntryCodeExistsAsync(string entryCode)
    {
        return _context.Set<Game>()
            .AnyAsync(t => t.EntryCode.Equals(entryCode));
    }
}