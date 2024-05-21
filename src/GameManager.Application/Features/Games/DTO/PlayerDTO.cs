namespace GameManager.Application.Features.Games.DTO;

public class PlayerDTO
{
    public Guid Id { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public PlayerState State { get; set; }

    public bool IsHost { get; set; }

    public IDictionary<Guid, int> TrackerValues { get; set; } = new Dictionary<Guid, int>();
}