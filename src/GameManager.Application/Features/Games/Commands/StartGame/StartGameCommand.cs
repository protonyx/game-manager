using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid GameId { get; }

    public StartGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}