using GameManager.Application.DTO;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandResponse : ValidateCommandResponseBase
{
    public PlayerDTO? Player { get; set; }
}