using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : ICommand
{
    public Guid GameId { get; }

    public EndTurnCommand(Guid gameId)
    {
        GameId = gameId;
    }
}