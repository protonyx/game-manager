namespace GameManager.Application.DTO;

public class PlayerCredentialsDTO
{
    public Guid GameId { get; set; }
    
    public Guid PlayerId { get; set; }

    public string Token { get; set; } = string.Empty;
    
    public bool IsAdmin { get; set; }
}