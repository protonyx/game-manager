namespace GameManager.Domain.Entities;

public class CurrentTurnDetails
{
    public Guid PlayerId { get; private set; }

    public DateTime StartTime { get; private set; }

    protected CurrentTurnDetails()
    {

    }

    public CurrentTurnDetails(Player player)
    {
        PlayerId = player.Id;
        StartTime = DateTime.UtcNow;
    }
}