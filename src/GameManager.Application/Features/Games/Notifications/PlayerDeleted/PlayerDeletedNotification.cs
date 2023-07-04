using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerDeleted;

public class PlayerDeletedNotification : INotification
{
    public Guid GameId { get; }

    public Guid PlayerId { get; }

    public PlayerDeletedNotification(Player player)
    : this(player.GameId, player.Id)
    {
    }

    public PlayerDeletedNotification(Guid gameId, Guid playerId)
    {
        GameId = gameId;
        PlayerId = playerId;
    }
}