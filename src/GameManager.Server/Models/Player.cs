namespace GameManager.Server.Models;

public class Player
{
    public Guid Id { get; set; }
    
    public Guid GameId { get; set; }

    public int Order { get; set; }
    
    public string Name { get; set; }
    
    public string Token { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public DateTime? LastHeartbeat { get; set; }
    
    public Game Game { get; set; }

    public ICollection<TrackerValue> TrackerValues { get; set; }
}