using System.Collections.Concurrent;
using System.Security.Claims;
using GameManager.Application.Contracts;

namespace GameManager.Application.Authorization;

public class PlayerIdentityBuilder : IPlayerIdentityBuilder
{

    private readonly string _authenticationType;

    private const string RoleClaim = "role";

    private readonly ConcurrentBag<Claim> _claims = new();

    public PlayerIdentityBuilder(string authenticationType = "internal")
    {
        _authenticationType = authenticationType;
    }

    public IPlayerIdentityBuilder AddGameId(Guid gameId)
    {
        _claims.Add(new Claim(GameManagerClaimTypes.GameId, gameId.ToString()));

        return this;
    }

    public IPlayerIdentityBuilder AddPlayerId(Guid playerId)
    {
        _claims.Add(new Claim(GameManagerClaimTypes.PlayerId, playerId.ToString()));

        return this;
    }

    public IPlayerIdentityBuilder AddHostRole()
    {
        _claims.Add(new Claim(RoleClaim, GameManagerRoles.Host));

        return this;
    }

    public IPlayerIdentityBuilder AddAdminRole()
    {
        _claims.Add(new Claim(RoleClaim, GameManagerRoles.Admin));

        return this;
    }

    public ClaimsIdentity Build()
    {
        return new ClaimsIdentity(_claims, roleType: RoleClaim, nameType: ClaimTypes.Name,
            authenticationType: _authenticationType);
    }

    public static ClaimsPrincipal CreatePrincipal(Action<IPlayerIdentityBuilder> builderAction)
    {
        var builder = new PlayerIdentityBuilder();
        builderAction.Invoke(builder);

        var identity = builder.Build();

        return new ClaimsPrincipal(identity);
    }
}