using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Mappers;
using GameManager.Application.Messages;

namespace GameManager.Application.Features.Games.Notifications.PlayerCreated;

public class PlayerCreatedNotificationHandler : INotificationHandler<PlayerCreatedNotification>
{
    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly DtoMapper _mapper;

    public PlayerCreatedNotificationHandler(IGameClientNotificationService gameClientNotificationService, DtoMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }

    public Task Handle(PlayerCreatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        var dto = _mapper.PlayerToDto(player);

        var message = new PlayerJoinedMessage()
        {
            GameId = player.GameId,
            Player = dto
        };

        return _gameClientNotificationService.PlayerJoined(message, cancellationToken);
    }
}