namespace GameManager.Application.Features.Games.DTO;

public class TurnDTO
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int DurationSeconds { get; set; }
}