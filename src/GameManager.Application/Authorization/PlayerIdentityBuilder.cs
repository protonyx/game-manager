using System.Security.Claims;
using GameManager.Application.Contracts;

namespace GameManager.Application.Authorization;

public class PlayerIdentityBuilder : IPlayerIdentityBuilder
{
    private readonly ClaimsIdentity _identity;
    
    public PlayerIdentityBuilder()
    {
        _identity = new ClaimsIdentity();
    }

    public IPlayerIdentityBuilder AddGameId(Guid gameId)
    {
        _identity.AddClaim(new Claim(GameManagerClaimTypes.GameId, gameId.ToString()));
        
        return this;
    }

    public IPlayerIdentityBuilder AddPlayerId(Guid playerId)
    {
        _identity.AddClaim(new Claim(GameManagerClaimTypes.PlayerId, playerId.ToString()));
        
        return this;
    }

    public IPlayerIdentityBuilder AddAdminRole()
    {
        _identity.AddClaim(new Claim("role", GameManagerClaimTypes.AdminRole));
        
        return this;
    }
    
    public ClaimsIdentity Build()
    {
        return _identity;
    }

    public static ClaimsPrincipal CreatePrincipal(Action<IPlayerIdentityBuilder> builderAction)
    {
        var builder = new PlayerIdentityBuilder();
        builderAction.Invoke(builder);

        var identity = builder.Build();

        return new ClaimsPrincipal(identity);
    }
}