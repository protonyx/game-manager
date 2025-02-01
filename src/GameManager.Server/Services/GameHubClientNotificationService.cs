using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Services;

public class GameHubClientNotificationService : IGameClientNotificationService
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameHubClientNotificationService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task GameStateChanged(GameStateChangedMessage message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(message.GameId.ToString())
            .SendAsync(nameof(IGameClientNotificationService.GameStateChanged),
                message,
                cancellationToken: cancellationToken);
    }

    public Task PlayerJoined(PlayerJoinedMessage message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(message.GameId.ToString())
            .SendAsync(nameof(IGameClientNotificationService.PlayerJoined),
                message,
                cancellationToken: cancellationToken);
    }

    public Task PlayerUpdated(PlayerUpdatedMessage message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(message.GameId.ToString())
            .SendAsync(nameof(IGameClientNotificationService.PlayerUpdated),
                message,
                cancellationToken: cancellationToken);
    }

    public Task PlayerLeft(PlayerLeftMessage message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(message.GameId.ToString())
            .SendAsync(nameof(IGameClientNotificationService.PlayerLeft),
                message,
                cancellationToken: cancellationToken);
    }

    public Task UpdateCredentials(PlayerCredentialsDTO message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(message.PlayerId.ToString())
            .SendAsync(nameof(IGameClientNotificationService.UpdateCredentials),
                message,
                cancellationToken: cancellationToken);
    }
}