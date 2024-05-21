using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;

namespace GameManager.Application.Contracts;

/// <summary>
/// Client for sending Game notifications from the server to players
/// </summary>
public interface IGameClientNotificationService
{
    Task PlayerJoined(PlayerJoinedMessage message, CancellationToken cancellationToken);

    Task GameStateChanged(GameStateChangedMessage message, CancellationToken cancellationToken);

    Task PlayerUpdated(PlayerUpdatedMessage message, CancellationToken cancellationToken);

    Task PlayerLeft(PlayerLeftMessage message, CancellationToken cancellationToken);

    Task UpdateCredentials(PlayerCredentialsDTO message, CancellationToken cancellationToken);
}