using GameManager.Application.Contracts.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommand : IRequest<ICommandResponse>
{
}