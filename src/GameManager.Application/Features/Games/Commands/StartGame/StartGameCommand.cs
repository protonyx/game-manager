using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommand : IRequest<UnitResult<CommandError>>
{
    public Guid GameId { get; }

    public StartGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}