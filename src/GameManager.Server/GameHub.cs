using GameManager.Server.Data;
using GameManager.Server.Messages;
using GameManager.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server;

public class GameHub : Hub<IGameHubClient>
{
    private readonly GameStateService _gameStateService;

    public GameHub(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public override async Task OnConnectedAsync()
    {
        // Get the playerId from the authentication token
        //Context.User.FindFirst("playerId");
        // Lookup the game for that player
        // Add the connection to a group named with the Group ID
        //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
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