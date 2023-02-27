using System.Security.Claims;

namespace GameManager.Server;

public static class PlayerAuthorizationExtensions
{
    public static Guid? GetGameId(this ClaimsPrincipal user)
    {
        var sidClaim = user.FindFirst("sid");
        
        if (sidClaim != null && Guid.TryParse(sidClaim.Value, out Guid gameId))
        {
            return gameId;
        }

        return null;
    }
    
    public static Guid? GetPlayerId(this ClaimsPrincipal user)
    {
        var nameClaim = user.FindFirst("name");

        if (nameClaim != null && Guid.TryParse(nameClaim.Value, out Guid playerId))
        {
            return playerId;
        }

        return null;
    }
}