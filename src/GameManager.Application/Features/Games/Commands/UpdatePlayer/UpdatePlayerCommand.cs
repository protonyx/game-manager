using GameManager.Application.Commands;
using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommand : IRequest<ICommandResponse>
{
    public Guid PlayerId { get; set; }
    
    public PlayerDTO? Player { get; set; }
}