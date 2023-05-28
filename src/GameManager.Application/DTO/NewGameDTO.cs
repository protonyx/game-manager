using GameManager.Server.Models;

namespace GameManager.Server.DTO;

public class NewGameDTO
{
    public string Name { get; set; }
    
    public GameOptionsDTO Options { get; set; }

    public ICollection<TrackerDTO> Trackers { get; set; } = new List<TrackerDTO>();
}