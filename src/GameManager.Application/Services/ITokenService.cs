namespace GameManager.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid gameId, Guid playerId, bool isAdmin);
}