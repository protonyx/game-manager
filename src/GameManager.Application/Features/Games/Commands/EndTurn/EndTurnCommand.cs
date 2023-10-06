using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid GameId { get; }

    public EndTurnCommand(Guid gameId)
    {
        GameId = gameId;
    }
}