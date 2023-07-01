using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayerTurns;

public class GetPlayerTurnsQuery : IRequest<ICollection<TurnDTO>>
{
    public Guid PlayerId { get; set; }

    public GetPlayerTurnsQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}