using AutoMapper;
using GameManager.Application.Messages;
using GameManager.Application.Services;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerDeleted;

public class PlayerDeletedNotificationHandler : INotificationHandler<PlayerDeletedNotification>
{
    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly IMapper _mapper;

    public PlayerDeletedNotificationHandler(IGameClientNotificationService gameClientNotificationService, IMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }

    public Task Handle(PlayerDeletedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        
        var message = new PlayerLeftMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id
        };

        return _gameClientNotificationService.PlayerLeft(message, cancellationToken);
    }
}