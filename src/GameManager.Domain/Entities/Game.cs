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
            return Result.Failure("Invalid game state");
        }

        State = GameState.Complete;
        CompletedDate = DateTime.UtcNow;
        CurrentTurn = null;
        EntryCode = null;
        LastModified = DateTime.UtcNow;
        UpdateETag();

        return Result.Success();
    }

    public void SetCurrentTurn(Player currentPlayer)
    {
        CurrentTurn = new CurrentTurnDetails(currentPlayer);
        LastModified = DateTime.UtcNow;
        UpdateETag();
    }

    public Result AddTracker(string name, int startingValue)
    {
        var trackerOrError = Tracker.Create(this, name, startingValue);

        if (trackerOrError.IsFailure)
            return Result.Failure($"Failed to add tracker: {trackerOrError.Error}");
        
        this._trackers.Add(trackerOrError.Value);
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