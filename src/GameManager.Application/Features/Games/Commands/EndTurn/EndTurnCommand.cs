using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : IRequest<UnitResult<CommandError>>
{
    public Guid GameId { get; }

    public EndTurnCommand(Guid gameId)
    {
        GameId = gameId;
    }
}