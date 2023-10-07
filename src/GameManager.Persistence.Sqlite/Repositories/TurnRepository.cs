using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class TurnRepository : BaseRepository<Turn>, ITurnRepository
{
    public TurnRepository(GameContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Turn>> GetTurnsByPlayerId(Guid playerId, CancellationToken cancellationToken = default)
    {
        var turns = await _context.Set<Turn>()
            .AsQueryable()
            .AsNoTracking()
            .Where(t => t.PlayerId == playerId)
            .ToListAsync(cancellationToken);

        return turns;
    }

    public async Task<IReadOnlyList<Turn>> GetTurnsByGameId(Guid gameId, CancellationToken cancellationToken = default)
    {
        var turns = await _context.Set<Turn>()
            .AsQueryable()
            .AsNoTracking()
            .Where(t => t.Player.GameId == gameId)
            .ToListAsync(cancellationToken);

        return turns;
    }
}