using AutoMapper;
using GameManager.Application.Contracts;
using GameManager.Application.Messages;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerDeleted;

public class PlayerDeletedNotificationHandler : INotificationHandler<PlayerDeletedNotification>
{
    private readonly IGameClientNotificationService _gameClientNotificationService;

    public PlayerDeletedNotificationHandler(IGameClientNotificationService gameClientNotificationService)
    {
        _gameClientNotificationService = gameClientNotificationService;
    }

    public Task Handle(PlayerDeletedNotification notification, CancellationToken cancellationToken)
    {
        var message = new PlayerLeftMessage()
        {
            GameId = notification.GameId,
            PlayerId = notification.PlayerId
        };

        return _gameClientNotificationService.PlayerLeft(message, cancellationToken);
    }
}