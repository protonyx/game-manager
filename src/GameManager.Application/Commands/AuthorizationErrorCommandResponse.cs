using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Commands;

public class AuthorizationErrorCommandResponse : ICommandResponse
{
    public string Reason { get; }

    public AuthorizationErrorCommandResponse(string reason = "")
    {
        Reason = reason;
    }
}