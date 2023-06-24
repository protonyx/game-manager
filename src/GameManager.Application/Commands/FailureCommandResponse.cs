namespace GameManager.Application.Commands;

public class FailureCommandResponse : ICommandResponse
{
    public string Reason { get; }

    public FailureCommandResponse(string reason)
    {
        Reason = reason;
    }
}