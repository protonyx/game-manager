using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;

namespace GameManager.Application.Features.Games.Notifications.PlayerConnected;

public class PlayerConnectedNotificationHandler : INotificationHandler<PlayerConnectedNotification>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMediator _mediator;

    public PlayerConnectedNotificationHandler(
        IPlayerRepository playerRepository,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _mediator = mediator;
    }

    public async Task Handle(PlayerConnectedNotification notification, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(notification.PlayerId, cancellationToken);

        if (player == null)
        {
            return;
        }

        player.AddConnection(notification.ConnectionId);

        await _playerRepository.UpdateAsync(player, cancellationToken);

        await _mediator.Publish(new PlayerUpdatedNotification(player), cancellationToken);
    }
}