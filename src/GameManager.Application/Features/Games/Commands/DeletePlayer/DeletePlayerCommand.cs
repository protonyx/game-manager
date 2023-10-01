using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommand : IRequest<UnitResult<CommandError>>
{
    public Guid PlayerId { get; }

    public DeletePlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}