using System.Security.Claims;

namespace GameManager.Application.Contracts;

public interface ITokenService
{
    string GenerateToken(ClaimsIdentity identity);
}