using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayer;

public class GetPlayerQuery : IRequest<Result<PlayerDTO, ApplicationError>>
{
    public Guid PlayerId { get; }

    public GetPlayerQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}