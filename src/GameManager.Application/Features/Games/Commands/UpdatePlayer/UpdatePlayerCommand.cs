using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommand : IRequest<UpdatePlayerCommandResponse>
{
    public Guid PlayerId { get; set; }
    
    public PlayerDTO? Player { get; set; }
}