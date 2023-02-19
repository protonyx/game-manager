namespace GameManager.Server.Messages;

public class PlayerJoinedMessage
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }
}