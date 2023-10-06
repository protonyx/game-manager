using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQuery : IRequest<Result<GameSummaryDTO, ApplicationError>>
{
    public Guid GameId { get; }

    public GetGameSummaryQuery(Guid gameId)
    {
        GameId = gameId;
    }
}