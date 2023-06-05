using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommand : IRequest
{
    public Guid PlayerId { get; set; }

    public DeletePlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}