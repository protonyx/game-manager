namespace GameManager.Server.Models;

public class GameTrackerModel
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public string Name { get; set; }

    public int StartingValue { get; set; }
}