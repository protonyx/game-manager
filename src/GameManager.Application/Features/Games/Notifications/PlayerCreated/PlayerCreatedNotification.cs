using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.PlayerCreated;

public class PlayerCreatedNotification : INotification
{
    public Player Player { get; }

    public PlayerCreatedNotification(Player player)
    {
        Player = player;
    }
}