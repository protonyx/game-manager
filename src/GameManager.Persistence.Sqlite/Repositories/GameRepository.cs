using System.Text;
using GameManager.Application.Data;
using GameManager.Server.Models;
using GameManager.Server.Notifications;
using GameManager.Server.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Data;

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

        return await base.CreateAsync(game);
    }

    public override async Task<Game?> GetByIdAsync(Guid gameId)
    {
        IQueryable<Game> queryable = _context.Games
            .AsQueryable()
            .AsNoTracking()
            .Include(t => t.Options)
            .Include(t => t.Trackers);

        var game = await queryable.Where(t => t.Id == gameId)
            .FirstOrDefaultAsync();

        return game;
    }

    public async Task<Game?> UpdateGameCurrentTurnAsync(Guid gameId, Guid playerId)
    {
        var game = await _context.Games.FindAsync(gameId);

        if (game == null)
        {
            return null;
        }

        game.CurrentTurnPlayerId = playerId;
        game.LastTurnStartTime = DateTime.Now;

        await _context.SaveChangesAsync();

        await _mediator.Publish(new GameUpdatedNotification(game));

        return game;
    }

    public async Task<Game?> GetGameByEntryCodeAsync(string entryCode)
    {
        var game = await _context.Games
            .Where(t => t.EntryCode == entryCode.ToUpper())
            .FirstOrDefaultAsync();

        return game;
    }

}