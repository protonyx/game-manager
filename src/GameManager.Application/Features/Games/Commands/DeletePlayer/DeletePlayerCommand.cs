using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid PlayerId { get; }

    public DeletePlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}