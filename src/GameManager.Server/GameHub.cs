using GameManager.Server.Data;
using GameManager.Server.Messages;
using GameManager.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server;

[Authorize]
public class GameHub : Hub<IGameHubClient>
{
    private readonly GameStateService _gameStateService;

    public GameHub(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User != null)
        {
            // Get the gameId from the authentication token
            var gameId = Context.User.FindFirst("sid");
            
            // Add the connection to a group named with the Group ID
            if (gameId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            }
        }
        
        await base.OnConnectedAsync();
    }

    public async Task RegisterClient(Guid gameId)
    {
        var connectionId = Context.ConnectionId;
        await Groups.AddToGroupAsync(connectionId, gameId.ToString());
    }

    public async Task Heartbeat(Guid playerId)
    {
        await _gameStateService.UpdatePlayerHeartbeat(playerId);
    }
    
    public async Task EndTurn(Guid gameId, Guid playerId)
    {
        var currentPlayerTurn = await _gameStateService.GetCurrentTurn(gameId);

        if (currentPlayerTurn != playerId)
        {
            // Only the current player can end the turn
            return;
        }

        await _gameStateService.AdvanceTurn(gameId);
        
        await Clients.All.GameStateChanged(new GameStateChangedMessage()
        {
            GameId = gameId
        });
    }
}