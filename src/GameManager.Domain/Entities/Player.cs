namespace GameManager.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }
    
    public Guid GameId { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
    
    public bool IsAdmin { get; set; }
    
    public DateTime? LastHeartbeat { get; set; }

    public ICollection<TrackerValue> TrackerValues { get; set; } = new List<TrackerValue>();

    public ICollection<Turn> Turns { get; set; } = new List<Turn>();
}