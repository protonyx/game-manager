using GameManager.Server.DTO;

namespace GameManager.Server.Messages;

public class PlayerJoinedMessage
{
    public Guid GameId { get; set; }

    public PlayerDTO Player { get; set; }
}