using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayerList;

public class GetPlayerListQuery : IRequest<Result<IReadOnlyList<PlayerDTO>, ApplicationError>>
{
    public Guid GameId { get; }

    public GetPlayerListQuery(Guid gameId)
    {
        GameId = gameId;
    }
}