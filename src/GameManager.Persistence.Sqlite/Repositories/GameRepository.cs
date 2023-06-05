using GameManager.Application.Data;
using GameManager.Application.Features.Games.Notifications;
using GameManager.Application.Services;
using GameManager.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    private readonly IMediator _mediator;
    
    private const int EntryCodeLength = 4;

    public GameRepository(GameContext context, IMediator mediator)
        : base(context)
    {
        _mediator = mediator;
    }

    public override async Task<Game> CreateAsync(Game game)
    {
        game.Id = Guid.NewGuid();
        game.EntryCode = EntryCodeFactory.Create(EntryCodeLength);
        game.CreatedDate = DateTime.Now;

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

}