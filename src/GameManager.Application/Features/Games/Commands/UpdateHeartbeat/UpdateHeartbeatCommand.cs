using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : IRequest
{
    public Guid PlayerId { get; set; }
}