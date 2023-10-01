using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class TurnRepository : BaseRepository<Turn>, ITurnRepository
{
    public TurnRepository(GameContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Turn>> GetTurnsByPlayerId(Guid playerId)
    {
        var turns = await _context.Set<Turn>()
            .AsQueryable()
            .AsNoTracking()
            .Where(t => t.PlayerId == playerId)
            .ToListAsync();

        return turns;
    }
}