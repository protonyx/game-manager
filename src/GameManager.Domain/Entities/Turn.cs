namespace GameManager.Domain.Entities;

public class Turn
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }
    
    public virtual Player Player { get; set; }
}