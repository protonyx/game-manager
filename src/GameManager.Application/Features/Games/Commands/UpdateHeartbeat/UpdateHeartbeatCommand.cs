using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : IRequest<UnitResult<CommandError>>
{
    public Guid PlayerId { get; }

    public UpdateHeartbeatCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}