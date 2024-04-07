using GameManager.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class GameAuthorizationHandler : AuthorizationHandler<GameAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GameAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GameAuthorizationRequirement requirement)
    {
        var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
        var user = _httpContextAccessor.HttpContext.User;
        
        if (!string.IsNullOrWhiteSpace(requirement.GameIdRouteParameterName)
            && routeValues.TryGetValue(requirement.GameIdRouteParameterName, out object? value)
            && value != null
            && Guid.TryParse(value.ToString(), out Guid gameId)
            && (requirement.Modify
                ? user.IsAuthorizedToModifyGame(gameId)
                : user.IsAuthorizedToViewGame(gameId)))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}