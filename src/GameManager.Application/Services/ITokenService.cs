namespace GameManager.Server.Services;

public interface ITokenService
{
    string GenerateToken(Guid gameId, Guid playerId, bool isAdmin);
}