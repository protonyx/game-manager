using GameManager.Application.Features.Games.DTO;

namespace GameManager.Server.Messages;

public class PlayerStateChangedMessage
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }

    public PlayerDTO? Player { get; set; }
}