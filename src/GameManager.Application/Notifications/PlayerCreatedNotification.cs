using GameManager.Server.Models;
using MediatR;

namespace GameManager.Server.Notifications;

public class PlayerCreatedNotification : INotification
{
    public Player Player { get; }

    public PlayerCreatedNotification(Player player)
    {
        Player = player;
    }
}