using AutoMapper;
using GameManager.Server.DTO;
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
        var gameId = Context.User?.GetGameId();
        
        if (gameId.HasValue)
        {
            // Add the connection to a group named with the Group ID
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.Value.ToString());
        }
        
        await base.OnConnectedAsync();
    }

    public async Task Heartbeat()
    {
        var playerId = Context.User?.GetPlayerId();

        if (playerId.HasValue)
        {
            await _gameStateService.UpdatePlayerHeartbeat(playerId.Value);
        }
    }
    
    public async Task EndTurn()
    {
        var gameId = Context.User?.GetGameId();
        var playerId = Context.User?.GetPlayerId();

        if (!gameId.HasValue || !playerId.HasValue)
        {
            return;
        }
        
        var currentPlayerTurn = await _gameStateService.GetCurrentTurn(gameId.Value);

        if (currentPlayerTurn != playerId)
        {
            // Only the current player can end the turn
            return;
        }

        await _gameStateService.AdvanceTurn(gameId.Value);
    }
}