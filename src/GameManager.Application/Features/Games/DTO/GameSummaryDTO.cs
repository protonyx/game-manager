namespace GameManager.Application.Features.Games.DTO;

public class GameSummaryDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public ICollection<PlayerSummaryDTO> Players { get; set; } = new List<PlayerSummaryDTO>();
    
    public DateTime CreatedDate { get; set; }

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }
}