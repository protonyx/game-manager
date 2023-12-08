using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid PlayerId { get; }

    public UpdateHeartbeatCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}