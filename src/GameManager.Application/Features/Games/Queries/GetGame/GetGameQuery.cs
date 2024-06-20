using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public class GetGameQuery : IQuery<GetGameQueryResponse>
{
    public Guid GameId { get; }

    public GetGameQuery(Guid gameId)
    {
        GameId = gameId;
    }
}