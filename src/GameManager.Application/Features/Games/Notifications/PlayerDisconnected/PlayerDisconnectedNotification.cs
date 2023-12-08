namespace GameManager.Application.Features.Games.Notifications.PlayerDisconnected;

public class PlayerDisconnectedNotification : INotification
{
    public Guid PlayerId { get; }

    public PlayerDisconnectedNotification(Guid playerId)
    {
        PlayerId = playerId;
    }
}