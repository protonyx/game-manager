using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommand : IRequest<Result<GameDTO, ApplicationError>>
{
    public CreateGameDTO Game { get; }

    public CreateGameCommand(CreateGameDTO game)
    {
        Game = game;
    }
}