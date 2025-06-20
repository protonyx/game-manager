using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayerList;

public class GetPlayerListQuery : IQuery<IReadOnlyList<PlayerDTO>>
{
    public Guid GameId { get; }

    public GetPlayerListQuery(Guid gameId)
    {
        GameId = gameId;
    }
}