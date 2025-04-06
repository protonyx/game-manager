namespace GameManager.Server.Models;

public class TurnModel
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }
}