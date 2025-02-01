namespace GameManager.Server.Models;

public class PlayerTrackerValueModel
{
    public Guid TrackerId { get; set; }

    public string Name { get; set; }

    public int Value { get; set; }
}