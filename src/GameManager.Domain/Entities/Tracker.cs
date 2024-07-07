using GameManager.Domain.Common;

namespace GameManager.Domain.Entities;

public class Tracker : IEntity<Guid>
{
    public Guid Id { get; private set; }

    public Guid GameId { get; private set; }

    public string Name { get; private set; }

    public int StartingValue { get; private set; }

    private Tracker()
    {
        
    }

    private Tracker(Game game, string name, int startingValue)
    : this()
    {
        Id = Guid.NewGuid();
        GameId = game.Id;
        Name = name;
        StartingValue = startingValue;
    }

    public static Result<Tracker> Create(Game game, string name, int startingValue = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Tracker>("Name is required");

        return new Tracker(game, name, startingValue);
    }
}