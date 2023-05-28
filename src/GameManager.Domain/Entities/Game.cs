namespace GameManager.Server.Models;

public class Game
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string EntryCode { get; set; }

    public GameOptions Options { get; set; } = new GameOptions();
    
    public Guid? CurrentTurnPlayerId { get; set; }
    
    public DateTime? LastTurnStartTime { get; set; }

    public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
}