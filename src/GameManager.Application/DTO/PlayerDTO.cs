namespace GameManager.Application.DTO;

public class PlayerDTO
{
    public Guid Id { get; set; }
    
    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public IDictionary<Guid, int> TrackerValues { get; set; } = new Dictionary<Guid, int>();
}