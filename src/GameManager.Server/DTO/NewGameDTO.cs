using GameManager.Server.Models;

namespace GameManager.Server.DTO;

public class NewGameDTO
{
    public string Name { get; set; }
    
    public GameOptions Options { get; set; }

    public ICollection<Tracker> Trackers { get; set; } = new List<Tracker>();
}