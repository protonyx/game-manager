namespace GameManager.Application.Features.Games.Notifications.PlayerPromoted;

public class PlayerPromotedNotification : INotification
{
    public Player Player { get; set; }

    public PlayerPromotedNotification(Player player)
    {
        Player = player;
    }
}