using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid PlayerId { get; }

    public string ConnectionId { get; }

    public UpdateHeartbeatCommand(Guid playerId, string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }
}