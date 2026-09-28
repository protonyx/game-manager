using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite.Repositories;

public class PushSubscriptionRepository(GameContext context)
    : BaseRepository<PushSubscription>(context), IPushSubscriptionRepository
{
    public Task<PushSubscription?> GetByEndpointAsync(string endpoint, CancellationToken cancellationToken = default)
        => _context.Set<PushSubscription>()
            .FirstOrDefaultAsync(subscription => subscription.Endpoint == endpoint, cancellationToken);

    public async Task<IReadOnlyList<PushSubscription>> GetByPlayerIdAsync(
        Guid playerId,
        CancellationToken cancellationToken = default)
        => await _context.Set<PushSubscription>()
            .AsNoTracking()
            .Where(subscription => subscription.PlayerId == playerId)
            .ToListAsync(cancellationToken);

    public async Task DeleteByEndpointAsync(string endpoint, CancellationToken cancellationToken = default)
        => await _context.Set<PushSubscription>()
            .Where(subscription => subscription.Endpoint == endpoint)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task DeleteByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default)
        => await _context.Set<PushSubscription>()
            .Where(subscription => subscription.PlayerId == playerId)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task DeleteByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        => await _context.Set<PushSubscription>()
            .Where(subscription => subscription.GameId == gameId)
            .ExecuteDeleteAsync(cancellationToken);
}
