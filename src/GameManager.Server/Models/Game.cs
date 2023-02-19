namespace GameManager.Server.Models;

public class Game
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string EntryCode { get; set; }
    
    public GameOptions Options { get; set; }
    
    public Guid? CurrentTurnPlayerId { get; set; }

    public ICollection<Player> Players { get; set; } = new List<Player>();

    public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
}