using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerUpdated;

public class PlayerUpdatedNotification : INotification
{
    public Player Player { get; }

    public PlayerUpdatedNotification(Player player)
    {
        Player = player;
    }
}