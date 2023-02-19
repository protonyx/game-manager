namespace GameManager.Server.DTO;

public class PlayerJoinDTO
{
    public Guid PlayerId { get; set; }
    
    public string Name { get; set; }
    
    public string Token { get; set; }
}