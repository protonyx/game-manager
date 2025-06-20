using Microsoft.AspNetCore.Authorization;

namespace GameManager.Server.Authorization;

public static class AuthorizationExtensions
{
    public static AuthorizationPolicyBuilder CanViewGame(
        this AuthorizationPolicyBuilder builder,
        string gameIdRouteParameterName = "Id")
    {
        builder.Requirements.Add(new GameAuthorizationRequirement(gameIdRouteParameterName));

        return builder;
    }

    public static AuthorizationPolicyBuilder CanModifyGame(
        this AuthorizationPolicyBuilder builder,
        string gameIdRouteParameterName = "Id")
    {
        builder.Requirements.Add(new GameAuthorizationRequirement(gameIdRouteParameterName, true));

        return builder;
    }

    public static AuthorizationPolicyBuilder CanViewPlayer(
        this AuthorizationPolicyBuilder builder,
        string playerIdRouteParameterName = "Id")
    {
        builder.Requirements.Add(new PlayerAuthorizationRequirement(playerIdRouteParameterName, false));

        return builder;
    }

    public static AuthorizationPolicyBuilder CanModifyPlayer(
        this AuthorizationPolicyBuilder builder,
        string playerIdRouteParameterName = "Id")
    {
        builder.Requirements.Add(new PlayerAuthorizationRequirement(playerIdRouteParameterName, true));

        return builder;
    }
}