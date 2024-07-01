using GameManager.Application.Authorization;
using GameManager.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class PlayerAuthorizationHandler : AuthorizationHandler<PlayerAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IPlayerRepository _playerRepository;

    public PlayerAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IPlayerRepository playerRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _playerRepository = playerRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PlayerAuthorizationRequirement requirement)
    {
        var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
        var user = _httpContextAccessor.HttpContext.User;

        if (!string.IsNullOrWhiteSpace(requirement.PlayerIdRouteParameterName)
            && routeValues.TryGetValue(requirement.PlayerIdRouteParameterName, out object? value)
            && value != null
            && Guid.TryParse(value.ToString(), out Guid playerId))
        {
            // Retrieve the player upon which we are trying to perform an operation
            var player = await _playerRepository.GetByIdAsync(playerId);

            if (player is not { Active: true })
            {
                context.Fail(new AuthorizationFailureReason(this, "Player is not valid"));

                return;
            }

            if (requirement.Modify
                ? user.IsAuthorizedToModifyPlayer(playerId, player.GameId)
                : user.IsAuthorizedToViewPlayer(playerId, player.GameId))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "User is not authorized for this player"));
            }
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Unable to parse Player ID"));
        }
    }
}