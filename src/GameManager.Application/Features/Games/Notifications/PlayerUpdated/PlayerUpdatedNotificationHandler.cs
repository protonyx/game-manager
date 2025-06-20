using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;

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

    public async Task Handle(PlayerUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;

        // Do not notify if the player is no longer active
        if (!player.Active)
        {
            return;
        }

        var dto = _mapper.Map<PlayerDTO>(player);

        var message = new PlayerUpdatedMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id,
            Player = dto
        };

        await _gameClientNotificationService.PlayerUpdated(message, cancellationToken);
    }
}