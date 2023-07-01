namespace GameManager.Application.Features.Games.DTO;

public class GameDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public string EntryCode { get; set; } = string.Empty;

    public GameOptionsDTO Options { get; set; } = new GameOptionsDTO();
    
    public Guid? CurrentTurnPlayerId { get; set; }
    
    public DateTime? LastTurnStartTime { get; set; }

    public ICollection<TrackerDTO> Trackers { get; set; } = new List<TrackerDTO>();
}