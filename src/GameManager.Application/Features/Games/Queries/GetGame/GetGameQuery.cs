using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public class GetGameQuery : IRequest<GameDTO?>
{
    public Guid GameId { get; set; }

    public GetGameQuery(Guid gameId)
    {
        GameId = gameId;
    }
}