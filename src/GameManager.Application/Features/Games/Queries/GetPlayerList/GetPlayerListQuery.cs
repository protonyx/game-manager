using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayerList;

public class GetPlayerListQuery : IRequest<ICollection<PlayerDTO>>
{
    public Guid GameId { get; set; }

    public GetPlayerListQuery(Guid gameId)
    {
        GameId = gameId;
    }
}