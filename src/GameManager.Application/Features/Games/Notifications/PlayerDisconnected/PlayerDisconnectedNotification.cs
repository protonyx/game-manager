namespace GameManager.Application.Features.Games.Notifications.PlayerDisconnected;

public class PlayerDisconnectedNotification : INotification
{
    public Guid PlayerId { get; }

    public string ConnectionId { get; }

    public PlayerDisconnectedNotification(Guid playerId, string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }
}