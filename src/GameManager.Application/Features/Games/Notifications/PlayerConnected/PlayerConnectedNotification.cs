namespace GameManager.Application.Features.Games.Notifications.PlayerConnected;

public class PlayerConnectedNotification : INotification
{
    public Guid PlayerId { get; }

    public string ConnectionId { get; }

    public PlayerConnectedNotification(Guid playerId, string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }
}