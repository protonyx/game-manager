using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GameManager.Domain.Common;
using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public record Game : IEntity<Guid>
{
    public Guid Id { get; private set; }

    public GameName Name { get; private set; }

    public EntryCode? EntryCode { get; private set; }

    public GameState State { get; private set; }

    public GameOptions Options { get; private set; }

    public CurrentTurnDetails? CurrentTurn { get; private set; }

    private List<Tracker> _trackers = new();
    public IReadOnlyList<Tracker> Trackers => _trackers.ToList();

    public DateTime CreatedDate { get; private set; }

    public DateTime? StartedDate { get; private set; }

    public DateTime? CompletedDate { get; private set; }

    public DateTime? LastModified { get; private set; }

    public ETag ETag { get; private set; } = ETag.Empty();

    protected Game()
    {

    }

    public Game(GameName name, GameOptions options)
        : this()
    {
        Id = Guid.NewGuid();
        EntryCode = EntryCode.New();
        Name = name;
        State = GameState.Preparing;
        Options = options;
        CreatedDate = DateTime.UtcNow;
        UpdateETag();
    }

    public Result ChangeName(GameName name)
    {
        Name = name;
        UpdateETag();

        return Result.Success();
    }

    public Result RegenerateEntryCode()
    {
        if (State != GameState.Preparing)
        {
            return Result.Failure("Invalid game state");
        }

        var codeLength = EntryCode?.Value.Length ?? 4;
        EntryCode = EntryCode.New(codeLength);
        LastModified = DateTime.UtcNow;
        UpdateETag();

        return Result.Success();
    }

    public Result Start(Player startingPlayer)
    {
        if (State != GameState.Preparing)
        {
            return Result.Failure("Invalid game state");
        }

        State = GameState.InProgress;
        StartedDate = DateTime.UtcNow;
        SetCurrentTurn(startingPlayer);
        LastModified = DateTime.UtcNow;
        UpdateETag();

        return Result.Success();
    }

    public Result Complete()
    {
        if (State != GameState.InProgress)
        {
            // Return a validation error message (string) for the Result
            return Result.Failure("Game must be in progress to complete.");
        }

        State = GameState.Complete;
        CompletedDate = DateTime.UtcNow;
        CurrentTurn = null;
        EntryCode = null;
        LastModified = DateTime.UtcNow;
        UpdateETag();

        return Result.Success();
    }

    public Result<Turn> SetCurrentTurn(Player currentPlayer)
    {
        var previousTurn = CurrentTurn;
        var currentTime = DateTime.UtcNow;

        CurrentTurn = new CurrentTurnDetails(currentPlayer);
        LastModified = currentTime;
        UpdateETag();

        if (previousTurn == null)
        {
            return Result.Failure<Turn>("No previous turn");
        }

        return Turn.Create(previousTurn.PlayerId, previousTurn.StartTime, currentTime);
    }

    public bool CheckCurrentTurn(Player player)
    {
        return CurrentTurn != null && CurrentTurn.PlayerId == player.Id;
    }

    public Result AddTracker(string name, int startingValue)
    {
        var trackerNameOrError = TrackerName.From(name);

        if (trackerNameOrError.IsFailure)
        {
            return trackerNameOrError;
        }

        var trackerOrError = Tracker.Create(this, trackerNameOrError.Value, startingValue);

        if (trackerOrError.IsFailure)
        {
            return Result.Failure($"Failed to add tracker: {trackerOrError.Error}");
        }

        _trackers.Add(trackerOrError.Value);
        LastModified = DateTime.UtcNow;
        UpdateETag();

        return Result.Success();
    }

    private void UpdateETag()
    {
        var toChecksum = this with { ETag = ETag.Empty() };
        var checksumBytes = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(toChecksum));

        using var hash = SHA1.Create();
        var hashBytes = hash.ComputeHash(checksumBytes);
        ETag = ETag.From(Convert.ToHexString(hashBytes));
    }
}
