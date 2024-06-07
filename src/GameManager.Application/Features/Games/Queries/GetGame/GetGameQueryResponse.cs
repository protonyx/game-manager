using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public record GetGameQueryResponse
{
    public GameDTO Game { get; }

    public ETag ETag { get; }

    public GetGameQueryResponse(GameDTO game, ETag eTag)
    {
        Game = game;
        ETag = eTag;
    }
}