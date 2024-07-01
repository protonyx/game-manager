using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;

namespace GameManager.Application.Features.Games.Notifications.PlayerCreated;

public class PlayerCreatedNotificationHandler : INotificationHandler<PlayerCreatedNotification>
{
    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly IMapper _mapper;

    public PlayerCreatedNotificationHandler(IGameClientNotificationService gameClientNotificationService, IMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }

    public Task Handle(PlayerCreatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        var dto = _mapper.Map<PlayerDTO>(player);

        var message = new PlayerJoinedMessage()
        {
            GameId = player.GameId,
            Player = dto
        };

        return _gameClientNotificationService.PlayerJoined(message, cancellationToken);
    }
}