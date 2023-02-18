namespace GameManager.Server.DTO;

public class GameDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string EntryCode { get; set; }

    public ICollection<PlayerDTO> Players { get; set; }
    
    public ICollection<TrackerDTO> Trackers { get; set; }
}