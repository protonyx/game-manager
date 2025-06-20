using GameManager.Application.Authorization;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public class GameResourceAuthorizationHandler : AuthorizationHandler<GameAuthorizationRequirement, IResolverContext>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GameAuthorizationRequirement requirement,
        IResolverContext resource)
    {
        // GraphQL Query
        var idArg = resource.ArgumentOptional<Guid>("id");
        var user = context.User;

        if (user.IsAdmin())
        {
            context.Succeed(requirement);
        }
        else if (idArg.HasValue)
        {
            var gameId = idArg.Value;
            if (requirement.Modify
                    ? user.IsAuthorizedToModifyGame(gameId)
                    : user.IsAuthorizedToViewGame(gameId))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "User is not authorized for this game"));
            }
        }

        return Task.CompletedTask;
    }
}