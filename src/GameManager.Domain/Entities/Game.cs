namespace GameManager.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string EntryCode { get; set; } = string.Empty;

    public GameOptions Options { get; set; } = new GameOptions();
    
    public Guid? CurrentTurnPlayerId { get; set; }
    
    public DateTime? LastTurnStartTime { get; set; }

    public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();

    public DateTime CreatedDate { get; set; }
}