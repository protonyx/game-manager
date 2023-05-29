namespace GameManager.Domain.Entities;

public class GameOptions
{
    public Guid GameId { get; set; }
    
    public bool ShareOtherPlayerTrackers { get; set; } = true;
}