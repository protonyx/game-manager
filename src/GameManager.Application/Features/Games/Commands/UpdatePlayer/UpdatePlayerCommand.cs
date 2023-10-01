using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommand : IRequest<Result<PlayerDTO, CommandError>>
{
    public Guid PlayerId { get; set; }
    
    public PlayerDTO? Player { get; set; }
}