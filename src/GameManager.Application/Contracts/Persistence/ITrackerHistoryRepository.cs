namespace GameManager.Application.Contracts.Persistence;

public interface ITrackerHistoryRepository : IAsyncRepository<TrackerHistory>
{
    Task<IReadOnlyList<TrackerHistory>> GetHistoryByGameId(Guid gameId,
        CancellationToken cancellationToken = default);
}