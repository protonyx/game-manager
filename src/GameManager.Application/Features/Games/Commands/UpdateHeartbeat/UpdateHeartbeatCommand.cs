using GameManager.Application.Commands;
using GameManager.Application.Contracts.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommand : IRequest<ICommandResponse>
{
    public Guid PlayerId { get; set; }
}