using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : ICommand
{
    public Guid PlayerId { get; }

    public string ConnectionId { get; }

    public UpdateHeartbeatCommand(Guid playerId, string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }
}