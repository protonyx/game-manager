using GameManager.Application.Features.Games.DTO;

namespace GameManager.Server.Messages;

public class GameStateChangedMessage
{
    public Guid GameId { get; set; }

    public GameDTO? Game { get; set; }
}