using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid GameId { get; }

    public EndGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}