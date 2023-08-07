namespace GameManager.Domain.Entities;

public class CurrentTurnDetails
{
    public Guid PlayerId { get; set; }

    public DateTime StartTime { get; set; }
}