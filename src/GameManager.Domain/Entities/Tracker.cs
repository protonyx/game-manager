namespace GameManager.Server.Models;

public class Tracker
{
    public Guid Id { get; set; }
    
    public Guid GameId { get; set; }
    
    public string Name { get; set; }

    public int StartingValue { get; set; } = 0;
}