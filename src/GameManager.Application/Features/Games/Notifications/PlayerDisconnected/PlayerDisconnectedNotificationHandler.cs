using GameManager.Application.Features.Games.Notifications.PlayerUpdated;

namespace GameManager.Application.Features.Games.Notifications.PlayerDisconnected;

public class PlayerDisconnectedNotificationHandler : INotificationHandler<PlayerDisconnectedNotification>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMediator _mediator;

    public PlayerDisconnectedNotificationHandler(
        IPlayerRepository playerRepository,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _mediator = mediator;
    }

    public async Task Handle(PlayerDisconnectedNotification notification, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(notification.PlayerId, cancellationToken);

        if (player == null)
        {
            return;
        }
        
        player.SetState(PlayerState.Disconnected);

        await _playerRepository.UpdateAsync(player, cancellationToken);

        await _mediator.Publish(new PlayerUpdatedNotification(player), cancellationToken);
    }
}