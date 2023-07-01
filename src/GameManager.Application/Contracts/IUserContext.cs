using System.Security.Claims;

namespace GameManager.Application.Contracts;

public interface IUserContext
{
    public ClaimsPrincipal? User { get; }
}