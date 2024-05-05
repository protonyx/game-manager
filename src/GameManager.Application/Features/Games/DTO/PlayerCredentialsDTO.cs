namespace GameManager.Application.Features.Games.DTO;

public class PlayerCredentialsDTO
{
    public Guid GameId { get; set; }
    
    public Guid? PlayerId { get; set; }

    public string Token { get; set; } = string.Empty;
    
    public bool IsHost { get; set; }
}