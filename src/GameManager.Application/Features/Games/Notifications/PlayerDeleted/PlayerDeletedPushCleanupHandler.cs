using GameManager.Application.Contracts.Persistence;

namespace GameManager.Application.Features.Games.Notifications.PlayerDeleted;

public class PlayerDeletedPushCleanupHandler(IPushSubscriptionRepository subscriptions)
    : INotificationHandler<PlayerDeletedNotification>
{
    public Task Handle(PlayerDeletedNotification notification, CancellationToken cancellationToken)
        => subscriptions.DeleteByPlayerIdAsync(notification.PlayerId, cancellationToken);
}
