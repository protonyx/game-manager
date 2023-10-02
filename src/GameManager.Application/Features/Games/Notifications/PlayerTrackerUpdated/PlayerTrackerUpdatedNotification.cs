namespace GameManager.Application.Features.Games.Notifications.PlayerTrackerUpdated;

public class PlayerTrackerUpdatedNotification : INotification
{
    public Guid PlayerId { get; set; }

    public Guid TrackerId { get; set; }

    public int NewValue { get; set; }
}