namespace GameManager.Domain.Entities;

public class PlayerConnection
{
    public Guid PlayerId { get; private set; }

    public string ConnectionId { get; private set; }

    public DateTime ConnectedDate { get; private set; }

    public DateTime LastHeartbeat { get; private set; }

    public PlayerConnection(Guid playerId, string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
        ConnectedDate = DateTime.UtcNow;
        LastHeartbeat = ConnectedDate;
    }

    public void UpdateHeartbeat()
    {
        LastHeartbeat = DateTime.UtcNow;
    }

}