using GameManager.Application.Authorization;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Application.Features.Games.Commands.UpdateHeartbeat;
using GameManager.Application.Services;
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
            // Add the connection to a group named with the Group ID
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.Value.ToString());
        }

        var playerId = Context.User?.GetPlayerId();

        if (playerId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, playerId.Value.ToString());
        }
        
        await base.OnConnectedAsync();
    }

    public async Task Heartbeat()
    {
        var playerId = Context.User?.GetPlayerId();

        if (playerId.HasValue)
        {
            await _mediator.Send(new UpdateHeartbeatCommand()
            {
                PlayerId = playerId.Value
            });
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning(exception, "Player disconnected");
        
        return base.OnDisconnectedAsync(exception);
    }
}