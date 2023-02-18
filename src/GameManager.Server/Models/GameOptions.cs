namespace GameManager.Server.Models;

public class GameOptions
{
    public Guid GameId { get; set; }
    
    public bool ShareOtherPlayerTrackers { get; set; } = true;
}