using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : IRequestHandler<EndTurnCommand>
{
    public Task Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}