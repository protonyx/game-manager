using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayerTurns;

public class GetPlayerTurnsQuery : IRequest<Result<IReadOnlyList<TurnDTO>, ApplicationError>>
{
    public Guid PlayerId { get; }

    public GetPlayerTurnsQuery(Guid playerId)
    {
        PlayerId = playerId;
    }
}