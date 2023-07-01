using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayer;

public class GetPlayerQuery : IRequest<PlayerDTO?>
{
    public Guid PlayerId { get; set; }

    public GetPlayerQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}