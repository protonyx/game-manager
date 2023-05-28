using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : IRequest
{
    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }
}