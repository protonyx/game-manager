using GameManager.Server.Models;
using MediatR;

namespace GameManager.Server.Notifications;

public class PlayerUpdatedNotification : INotification
{
    public Player Player { get; }

    public PlayerUpdatedNotification(Player player)
    {
        Player = player;
    }
}