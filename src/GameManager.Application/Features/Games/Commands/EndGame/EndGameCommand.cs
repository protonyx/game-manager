using GameManager.Application.Contracts.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommand : IRequest<ICommandResponse>
{
    public Guid GameId { get; }

    public EndGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}