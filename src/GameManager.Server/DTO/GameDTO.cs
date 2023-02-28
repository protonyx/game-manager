namespace GameManager.Server.DTO;

public class GameDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string EntryCode { get; set; }
    
    public GameOptionsDTO Options { get; set; }
    
    public Guid? CurrentTurnPlayerId { get; set; }
    
    public ICollection<TrackerDTO> Trackers { get; set; }
}