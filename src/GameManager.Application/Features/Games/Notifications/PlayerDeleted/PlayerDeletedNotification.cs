using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerDeleted;

public class PlayerDeletedNotification : INotification
{
    public Player Player { get; }

    public PlayerDeletedNotification(Player player)
    {
        Player = player;
    }
}