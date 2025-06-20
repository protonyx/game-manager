using GameManager.Domain.Common;
using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public record Tracker : IEntity<Guid>
{
    public Guid Id { get; private set; }

    public Guid GameId { get; private set; }

    public required TrackerName Name { get; init; }

    public int StartingValue { get; private set; }

    private Tracker()
    {

    }

    public static Result<Tracker> Create(Game game, TrackerName name, int startingValue = 0)
    {
        return new Tracker()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            Name = name,
            StartingValue = startingValue
        };
    }
}
