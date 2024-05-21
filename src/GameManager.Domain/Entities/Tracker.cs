namespace GameManager.Domain.Entities;

public class Tracker
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int StartingValue { get; set; } = 0;
}