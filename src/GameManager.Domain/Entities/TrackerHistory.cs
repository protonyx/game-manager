namespace GameManager.Domain.Entities;

public class TrackerHistory
{
    public Guid Id { get; set; }
    
    public Guid PlayerId { get; set; }
    
    public Guid TrackerId { get; set; }

    public DateTime ChangedTime { get; set; }
    
    public int NewValue { get; set; }
    
    public virtual Player Player { get; set; }

    public virtual Tracker Tracker { get; set; }
}