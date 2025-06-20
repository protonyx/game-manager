using GameManager.Domain.Common;

namespace GameManager.Server.Models;

public class PlayerModel
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public PlayerState State { get; set; }

    public bool IsHost { get; set; }

    public DateTime JoinedDate { get; set; }

    public ICollection<PlayerTrackerValueModel> Trackers { get; set; } = new List<PlayerTrackerValueModel>();
}