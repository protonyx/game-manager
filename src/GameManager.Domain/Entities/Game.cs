using GameManager.Domain.Common;
using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public EntryCode? EntryCode { get; private set; }

    public GameState State { get; private set; }

    public GameOptions Options { get; private set; }
    
    public CurrentTurnDetails? CurrentTurn { get; private set; }

    private List<Tracker> _trackers = new();
    public IReadOnlyList<Tracker> Trackers => _trackers.ToList();

    public DateTime CreatedDate { get; private set; }

    public DateTime? StartedDate { get; private set; }

    public DateTime? CompletedDate { get; private set; }

    protected Game()
    {
        
    }
    
    public Game(string name, GameOptions options)
        : this()
    {
        Id = Guid.NewGuid();
        EntryCode = EntryCode.New();
        Name = name;
        State = GameState.Preparing;
        Options = options;
        CreatedDate = DateTime.UtcNow;
    }

    public Result RegenerateEntryCode()
    {
        if (State != GameState.Preparing)
        {
            return Result.Failure("Invalid game state");
        }

        var codeLength = EntryCode?.Value.Length ?? 4;
        EntryCode = EntryCode.New(codeLength);

        return Result.Success();
    }

    public void Start(Player startingPlayer)
    {
        if (State != GameState.Preparing)
        {
            throw new InvalidOperationException("Invalid game state");
        }

        State = GameState.InProgress;
        StartedDate = DateTime.UtcNow;
        SetCurrentTurn(startingPlayer);
    }

    public void Complete()
    {
        if (State != GameState.InProgress)
        {
            throw new InvalidOperationException("Invalid game state");
        }

        State = GameState.Complete;
        CompletedDate = DateTime.UtcNow;
        CurrentTurn = null;
        EntryCode = null;
    }

    public void SetCurrentTurn(Player currentPlayer)
    {
        CurrentTurn = new CurrentTurnDetails(currentPlayer);
    }

    public void AddTracker(Tracker tracker)
    {
        this._trackers.Add(tracker);
    }
}