using GameManager.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class PlayerAuthorizationHandler : AuthorizationHandler<PlayerAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PlayerAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlayerAuthorizationRequirement requirement)
    {
        var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
        var user = _httpContextAccessor.HttpContext.User;
        
        if (!string.IsNullOrWhiteSpace(requirement.PlayerIdRouteParameterName)
            && routeValues.TryGetValue(requirement.PlayerIdRouteParameterName, out object? value)
            && value != null
            && Guid.TryParse(value.ToString(), out Guid playerId)
            && (requirement.Modify
                ? user.IsAuthorizedToModifyPlayer(playerId)
                : user.IsAuthorizedToViewPlayer(playerId)))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}