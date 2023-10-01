using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class ReorderPlayersCommand : IRequest<UnitResult<CommandError>>
{
    public Guid GameId { get; }

    public IList<Guid> PlayerIds { get; }

    public ReorderPlayersCommand(Guid gameId, ICollection<Guid> playerIds)
    {
        GameId = gameId;
        PlayerIds = playerIds.ToList();
    }
}