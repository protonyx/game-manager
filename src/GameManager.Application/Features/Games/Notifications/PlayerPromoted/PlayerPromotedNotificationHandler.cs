using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Notifications.PlayerPromoted;

public class PlayerPromotedNotificationHandler : INotificationHandler<PlayerPromotedNotification>
{
    private readonly IGameClientNotificationService _notificationService;

    private readonly ITokenService _tokenService;

    public PlayerPromotedNotificationHandler(
        IGameClientNotificationService notificationService,
        ITokenService tokenService)
    {
        _notificationService = notificationService;
        _tokenService = tokenService;
    }

    public async Task Handle(PlayerPromotedNotification notification, CancellationToken cancellationToken)
    {
        // Generate player token
        var identityBuilder = new PlayerIdentityBuilder();
        identityBuilder.AddGameId(notification.Player.GameId)
            .AddPlayerId(notification.Player.Id);

        if (notification.Player.IsHost)
        {
            identityBuilder.AddHostRole();
        }

        var token = _tokenService.GenerateToken(identityBuilder.Build());

        var playerCredentials = new PlayerCredentialsDTO
        {
            GameId = notification.Player.GameId,
            PlayerId = notification.Player.Id,
            Token = token,
            IsHost = notification.Player.IsHost
        };

        await _notificationService.UpdateCredentials(playerCredentials, cancellationToken);
    }
}