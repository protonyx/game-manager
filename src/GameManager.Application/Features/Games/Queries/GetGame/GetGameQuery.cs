using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public class GetGameQuery : IRequest<Result<GameDTO, ApplicationError>>
{
    public Guid GameId { get; }

    public GetGameQuery(Guid gameId)
    {
        GameId = gameId;
    }
}