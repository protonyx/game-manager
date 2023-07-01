using System.Security.Claims;

namespace GameManager.Application.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetGameId(this ClaimsPrincipal user)
    {
        var sidClaim = user.FindFirst(GameManagerClaimTypes.GameId);
        
        if (sidClaim != null && Guid.TryParse(sidClaim.Value, out Guid gameId))
        {
            return gameId;
        }

        return null;
    }

    public static bool IsAuthorizedForGame(this ClaimsPrincipal user, Guid gameId)
    {
        return user.GetGameId() == gameId;
    }
    
    public static Guid? GetPlayerId(this ClaimsPrincipal user)
    {
        var nameClaim = user.FindFirst(GameManagerClaimTypes.PlayerId);

        if (nameClaim != null && Guid.TryParse(nameClaim.Value, out Guid playerId))
        {
            return playerId;
        }

        return null;
    }
    
    public static bool IsAuthorizedForPlayer(this ClaimsPrincipal user, Guid playerId)
    {
        return user.GetPlayerId() == playerId || user.IsInRole(GameManagerClaimTypes.AdminRole);
    }
}