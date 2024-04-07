using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class PlayerAuthorizationRequirement(
    string playerIdRouteParameterName = "Id",
    bool modify = false)
    : IAuthorizationRequirement
{
    public string PlayerIdRouteParameterName { get; } = playerIdRouteParameterName;

    public bool Modify { get; } = modify;
}