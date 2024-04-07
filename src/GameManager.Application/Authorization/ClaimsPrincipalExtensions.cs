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

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(GameManagerRoles.Admin);
    }

    public static bool HasGameClaim(this ClaimsPrincipal user, Guid gameId)
    {
        return user.HasClaim(GameManagerClaimTypes.GameId, gameId.ToString());
    }

    public static bool IsHostForGame(this ClaimsPrincipal user, Guid gameId)
    {
        return user.HasGameClaim(gameId) && user.IsInRole(GameManagerRoles.Host);
    }

    public static bool IsAuthorizedToViewGame(this ClaimsPrincipal user, Guid gameId)
    {
        return user.HasGameClaim(gameId) || user.IsAdmin();
    }

    public static bool IsAuthorizedToModifyGame(this ClaimsPrincipal user, Guid gameId)
    {
        return user.IsHostForGame(gameId) || user.IsAdmin();
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

    public static bool HasPlayerClaim(this ClaimsPrincipal user, Guid playerId)
    {
        return user.HasClaim(GameManagerClaimTypes.PlayerId, playerId.ToString());
    }

    public static bool IsAuthorizedToModifyPlayer(this ClaimsPrincipal user, Guid playerId)
    {
        return user.HasPlayerClaim(playerId) || user.IsAdmin();
    }

    public static bool IsAuthorizedToViewPlayer(this ClaimsPrincipal user, Guid playerId)
    {
        return user.HasPlayerClaim(playerId) || user.IsAdmin();
    }
}