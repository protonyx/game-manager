namespace GameManager.Application.DTO;

public class TrackerDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int StartingValue { get; set; }
}