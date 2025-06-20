using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayer;

public class GetPlayerQuery : IQuery<PlayerDTO>
{
    public Guid PlayerId { get; }

    public GetPlayerQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}