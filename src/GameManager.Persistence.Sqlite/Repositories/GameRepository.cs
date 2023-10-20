using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(GameContext context)
        : base(context)
    {
    }
    
    public async Task<IReadOnlyList<Game>> FindAsync(DateTime? olderThan = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = _context.Set<Game>()
            .AsQueryable()
            .AsNoTracking();

        if (olderThan.HasValue)
        {
            query = query.Where(t => t.CreatedDate < olderThan);
        }

        var games = await query.ToListAsync(cancellationToken);

        return games;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Game>()
            .Where(t => t.Id == id)
            .AnyAsync(cancellationToken);
    }

    public override Task<Game> UpdateAsync(Game entity, CancellationToken cancellationToken = default)
    {
        if (entity.CurrentTurn != null)
        {
            var entry = _context.Entry(entity.CurrentTurn);

            if (entry.State == EntityState.Detached)
            {
                _context.Attach(entity.CurrentTurn);
            }
        }
        
        return base.UpdateAsync(entity, cancellationToken);
    }

    public override async Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Game>().FindAsync(new object?[] {gameId}, cancellationToken);
    }

    public async Task<Game?> GetGameByEntryCodeAsync(EntryCode entryCode, CancellationToken cancellationToken = default)
    {
        var game = await _context.Set<Game>()
            .Where(t => t.EntryCode.Equals(entryCode))
            .FirstOrDefaultAsync(cancellationToken);

        return game;
    }

    public Task<bool> EntryCodeExistsAsync(EntryCode entryCode, CancellationToken cancellationToken = default)
    {
        return _context.Set<Game>()
            .AnyAsync(t => t.EntryCode.Equals(entryCode), cancellationToken);
    }
}