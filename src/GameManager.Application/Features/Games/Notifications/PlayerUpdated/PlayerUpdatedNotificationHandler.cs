using AutoMapper;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;
using GameManager.Application.Services;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerUpdated;

public class PlayerUpdatedNotificationHandler : INotificationHandler<PlayerUpdatedNotification>
{
    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly IMapper _mapper;

    public PlayerUpdatedNotificationHandler(IGameClientNotificationService gameClientNotificationService, IMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }
    
    public Task Handle(PlayerUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        var dto = _mapper.Map<PlayerDTO>(player);
        
        var message = new PlayerStateChangedMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id,
            Player = dto
        };

        return _gameClientNotificationService.PlayerStateChanged(message, cancellationToken);
    }
}