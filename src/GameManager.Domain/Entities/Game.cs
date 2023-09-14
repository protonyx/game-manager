using GameManager.Domain.Common;
using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    private string _entryCode = string.Empty;
    public EntryCode EntryCode {
        get => _entryCode;
        set => _entryCode = value.Value;
    }

    public GameState State { get; set; } = GameState.Preparing;

    public GameOptions Options { get; set; } = new GameOptions();
    
    public CurrentTurnDetails? CurrentTurn { get; set; }

    public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();

    public DateTime CreatedDate { get; set; }

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }
}