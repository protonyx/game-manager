using GameManager.Application.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : IRequest<ICommandResponse>
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }
}