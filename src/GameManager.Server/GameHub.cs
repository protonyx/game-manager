using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Application.Features.Games.Commands.UpdateHeartbeat;
using GameManager.Application.Features.Games.Notifications.PlayerConnected;
using GameManager.Application.Features.Games.Notifications.PlayerDisconnected;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server;

[Authorize]
public class GameHub : Hub<IGameClientNotificationService>
{
    private readonly IMediator _mediator;
    
    private readonly ILogger<GameHub> _logger;

    public GameHub(IMediator mediator, ILogger<GameHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var gameId = Context.User?.GetGameId();
        
        if (gameId.HasValue)
        {
            // Add the connection to a group named with the Game ID
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.Value.ToString());
        }

        var playerId = Context.User?.GetPlayerId();

        if (playerId.HasValue)
        {
            // Add the connection to a group named with the Player ID
            await Groups.AddToGroupAsync(Context.ConnectionId, playerId.Value.ToString());
            
            _logger.LogInformation("Player {PlayerId} connected", playerId.Value);
            
            var notification = new PlayerConnectedNotification(playerId.Value);
            await _mediator.Publish(notification);
        }
    }

    public async Task Heartbeat()
    {
        var playerId = Context.User?.GetPlayerId();

        if (playerId.HasValue)
        {
            await _mediator.Send(new UpdateHeartbeatCommand(playerId.Value));
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var playerId = Context.User?.GetPlayerId();
        if (playerId.HasValue)
        {
            _logger.LogInformation(exception, "Player {PlayerId} disconnected", playerId.Value);
            
            var notification = new PlayerDisconnectedNotification(playerId.Value);
            await _mediator.Publish(notification);
        }
    }
}