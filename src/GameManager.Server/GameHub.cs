using GameManager.Application.Authorization;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Application.Features.Games.Commands.UpdateHeartbeat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server;

[Authorize]
public class GameHub : Hub<IGameHubClient>
{
    private readonly IMediator _mediator;

    public GameHub(IMediator mediator)
    {
        _mediator = mediator;
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
    
    public async Task EndTurn()
    {
        var cmd = new EndTurnCommand();

        var resp = await _mediator.Send(cmd);
    }
}