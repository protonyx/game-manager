using GameManager.Application.Commands;
using GameManager.Application.Contracts.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommand : IRequest<ICommandResponse>
{
    public Guid PlayerId { get; set; }

    public DeletePlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}