namespace GameManager.Server.DTO;

public class PlayerCredentialsDTO
{
    public Guid GameId { get; set; }
    
    public Guid PlayerId { get; set; }
    
    public string Token { get; set; }
}