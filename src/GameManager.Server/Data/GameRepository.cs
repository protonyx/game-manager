using System.Text;
using GameManager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Data;

public class GameRepository
{
    private const string ValidEntryCodeCharacters = "ABCEFGHJKMNPQRTWXY0123456789";

    private const int EntryCodeLength = 4;

    private readonly GameContext _context;

    public GameRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Game> CreateGameAsync(Game game)
    {
        game.Id = Guid.NewGuid();
        game.EntryCode = GenerateEntryCode(EntryCodeLength);

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return game;
    }

    public async Task<Game?> GetGameById(Guid gameId)
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

        await _context.SaveChangesAsync();

        return game;
    }

    public async Task<Game?> GetGameByEntryCode(string entryCode)
    {
        var game = await _context.Games
            .Where(t => t.EntryCode == entryCode.ToUpper())
            .FirstOrDefaultAsync();

        return game;
    }

    private string GenerateEntryCode(int length)
    {
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            var idx = Random.Shared.Next() % ValidEntryCodeCharacters.Length;

            sb.Append(ValidEntryCodeCharacters[idx]);
        }

        return sb.ToString();
    }
}