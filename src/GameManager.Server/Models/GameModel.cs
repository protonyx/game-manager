using GameManager.Domain.Common;
using GameManager.Domain.Entities;

namespace GameManager.Server.Models;

public class GameModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public string EntryCode { get; set; } = string.Empty;

    public GameState State { get; set; }

    public GameOptions Options { get; set; }

    public PlayerModel? CurrentTurn { get; set; }

    public ICollection<GameTrackerModel> Trackers { get; set; } = new List<GameTrackerModel>();

    public IReadOnlyList<PlayerModel> Players { get; set; } = new List<PlayerModel>();
}