using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface IPushSubscriptionRepository : IAsyncRepository<PushSubscription>
{
    Task<PushSubscription?> GetByEndpointAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PushSubscription>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task DeleteByEndpointAsync(string endpoint, CancellationToken cancellationToken = default);
    Task DeleteByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task DeleteByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
}
