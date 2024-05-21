using GameManager.Domain.Common;

namespace GameManager.Application.Features.Games.DTO;

public class GameDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public string EntryCode { get; set; } = string.Empty;

    public GameState State { get; set; }

    public GameOptionsDTO Options { get; set; } = new GameOptionsDTO();

    public Guid? CurrentTurnPlayerId { get; set; }

    public DateTime? LastTurnStartTime { get; set; }

    public ICollection<TrackerDTO> Trackers { get; set; } = new List<TrackerDTO>();

    public DateTime CreatedDate { get; set; }

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }
}