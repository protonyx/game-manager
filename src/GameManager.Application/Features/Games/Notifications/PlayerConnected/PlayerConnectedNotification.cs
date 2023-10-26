namespace GameManager.Application.Features.Games.Notifications.PlayerConnected;

public class PlayerConnectedNotification : INotification
{
    public Guid PlayerId { get; }

    public PlayerConnectedNotification(Guid playerId)
    {
        PlayerId = playerId;
    }
}