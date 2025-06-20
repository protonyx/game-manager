using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayerTurns;

public class GetPlayerTurnsQuery : IQuery<IReadOnlyList<TurnDTO>>
{
    public Guid PlayerId { get; }

    public GetPlayerTurnsQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}