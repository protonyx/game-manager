using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class GameAuthorizationRequirement(
    string gameIdRouteParameterName = "Id",
    bool modify = false)
    : IAuthorizationRequirement
{
    public string GameIdRouteParameterName { get; } = gameIdRouteParameterName;

    public bool Modify { get; } = modify;
}