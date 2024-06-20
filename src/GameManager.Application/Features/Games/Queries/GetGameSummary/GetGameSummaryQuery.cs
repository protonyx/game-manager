using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQuery : IQuery<GameSummaryDTO>
{
    public Guid GameId { get; }

    public GetGameSummaryQuery(Guid gameId)
    {
        GameId = gameId;
    }
}