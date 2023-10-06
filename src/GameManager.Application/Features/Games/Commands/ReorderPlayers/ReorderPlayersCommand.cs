using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class ReorderPlayersCommand : IRequest<UnitResult<ApplicationError>>
{
    public Guid GameId { get; }

    public IList<Guid> PlayerIds { get; }

    public ReorderPlayersCommand(Guid gameId, ICollection<Guid> playerIds)
    {
        GameId = gameId;
        PlayerIds = playerIds.ToList();
    }
}