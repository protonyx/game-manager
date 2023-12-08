using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Messages;

public class PlayerUpdatedMessage
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }

    public PlayerDTO? Player { get; set; }
}