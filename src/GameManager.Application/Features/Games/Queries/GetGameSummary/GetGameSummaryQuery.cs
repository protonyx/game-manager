using GameManager.Application.Contracts.Queries;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQuery : IRequest<IQueryResponse<GameSummaryDTO>>
{
    public Guid GameId { get; }

    public GetGameSummaryQuery(Guid gameId)
    {
        GameId = gameId;
    }
}