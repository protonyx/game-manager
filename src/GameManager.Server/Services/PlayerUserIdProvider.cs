using GameManager.Application.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Services;

public class PlayerUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.GetPlayerId()?.ToString();
    }
}