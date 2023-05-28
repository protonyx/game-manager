using GameManager.Server.Models;
using MediatR;

namespace GameManager.Server.Notifications;

public class PlayerDeletedNotification : INotification
{
    public Player Player { get; }

    public PlayerDeletedNotification(Player player)
    {
        Player = player;
    }
}