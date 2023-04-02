namespace GameManager.Server.Messages;

public class PlayerLeftMessage
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }
}