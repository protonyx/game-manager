namespace GameManager.Domain.Entities;

public class TrackerValue
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid TrackerId { get; set; }

    public int Value { get; set; }
}