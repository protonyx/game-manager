using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommand : IRequest<UnitResult<CommandError>>
{
    public Guid GameId { get; }

    public EndGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}