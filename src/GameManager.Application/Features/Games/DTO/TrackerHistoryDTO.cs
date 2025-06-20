namespace GameManager.Application.Features.Games.DTO;

public class TrackerHistoryDTO
{
    public Guid TrackerId { get; set; }

    public DateTime ChangedTime { get; set; }

    public int NewValue { get; set; }

    public int? SecondsSinceGameStart { get; set; }
}