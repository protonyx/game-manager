namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandResponse : ValidateCommandResponseBase
{
    
    public Guid GameId { get; set; }
    
    public Guid PlayerId { get; set; }

    public string Token { get; set; } = string.Empty;
    
    public bool IsAdmin { get; set; }
}