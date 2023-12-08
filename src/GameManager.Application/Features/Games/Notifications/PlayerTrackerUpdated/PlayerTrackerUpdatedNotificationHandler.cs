namespace GameManager.Application.Features.Games.Notifications.PlayerTrackerUpdated;

public class PlayerTrackerUpdatedNotificationHandler : INotificationHandler<PlayerTrackerUpdatedNotification>
{
    private readonly ITrackerHistoryRepository _trackerHistoryRepository;

    public PlayerTrackerUpdatedNotificationHandler(ITrackerHistoryRepository trackerHistoryRepository)
    {
        _trackerHistoryRepository = trackerHistoryRepository;
    }

    public async Task Handle(PlayerTrackerUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var history = new TrackerHistory()
        {
            Id = Guid.NewGuid(),
            PlayerId = notification.PlayerId,
            TrackerId = notification.TrackerId,
            ChangedTime = DateTime.UtcNow,
            NewValue = notification.NewValue
        };

        await _trackerHistoryRepository.CreateAsync(history, cancellationToken);
    }
}