using GameManager.Application.Messages;

namespace GameManager.Application.Services;

/// <summary>
/// Client for sending Game notifications from the server to players
/// </summary>
public interface IGameClientNotificationService
{
    Task PlayerJoined(PlayerJoinedMessage message, CancellationToken cancellationToken);

    Task GameStateChanged(GameStateChangedMessage message, CancellationToken cancellationToken);

    Task PlayerStateChanged(PlayerStateChangedMessage message, CancellationToken cancellationToken);

    Task PlayerLeft(PlayerLeftMessage message, CancellationToken cancellationToken);
}