using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommand : ICommand
{
    public Guid GameId { get; }

    public EndGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}