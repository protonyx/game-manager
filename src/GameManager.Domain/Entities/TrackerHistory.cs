namespace GameManager.Domain.Entities;

public class TrackerHistory
{
    public Guid Id { get; set; }
    
    public Guid PlayerId { get; set; }
    
    public Guid TrackerId { get; set; }

    public DateTime ChangedTime { get; set; }
    
    public int NewValue { get; set; }
}