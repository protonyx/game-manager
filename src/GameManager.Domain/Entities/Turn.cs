using GameManager.Domain.Common;

namespace GameManager.Domain.Entities;

public record Turn : IEntity<Guid>
{
    public Guid Id { get; private set; }

    public Guid PlayerId { get; private set; }

    public DateTime StartTime { get; private set; }

    public DateTime EndTime { get; private set; }

    public TimeSpan Duration { get; private set; }

    public virtual Player? Player { get; private set; }

    private Turn()
    {

    }

    public static Result<Turn> Create(Guid playerId, DateTime startTime, DateTime endTime)
    {
        if (playerId == Guid.Empty)
        {
            return Result.Failure<Turn>("Player ID is required");
        }

        if (endTime < startTime)
        {
            return Result.Failure<Turn>("End time must be after start time");
        }

        var duration = endTime - startTime;

        return new Turn
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            StartTime = startTime,
            EndTime = endTime,
            Duration = duration
        };
    }
}
