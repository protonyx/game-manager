using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandResponse
{
    public GameDTO Game { get; }

    public ETag ETag { get; }

    public CreateGameCommandResponse(GameDTO game, ETag eTag)
    {
        Game = game;
        ETag = eTag;
    }
}