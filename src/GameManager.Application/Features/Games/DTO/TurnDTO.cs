namespace GameManager.Application.Features.Games.DTO;

public class TurnDTO
{
    public Guid? Id { get; set; }

    public Guid PlayerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int DurationSeconds { get; set; }
}