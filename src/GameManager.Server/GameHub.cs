using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server;

public class GameHub : Hub
{
    public async Task EndTurn(Guid playerId)
    {
        await Clients.Others.SendAsync("TurnEnded", playerId);
    }
}