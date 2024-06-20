using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommand : ICommand
{
    public Guid GameId { get; }

    public StartGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}