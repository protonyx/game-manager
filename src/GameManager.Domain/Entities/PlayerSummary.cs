namespace GameManager.Domain.Entities;

public class PlayerSummary : Player
{
    public ICollection<Turn> Turns { get; set; }

    public ICollection<TrackerHistory> TrackerHistory { get; set; }

    protected PlayerSummary()
    {
        
    }

    public PlayerSummary(Player player)
        : base(player)
    {
        
    }
}