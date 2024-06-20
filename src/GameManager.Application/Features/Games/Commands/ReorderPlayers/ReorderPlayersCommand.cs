using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class ReorderPlayersCommand : ICommand
{
    public Guid GameId { get; }

    public IList<Guid> PlayerIds { get; }

    public ReorderPlayersCommand(Guid gameId, ICollection<Guid> playerIds)
    {
        GameId = gameId;
        PlayerIds = playerIds.ToList();
    }
}