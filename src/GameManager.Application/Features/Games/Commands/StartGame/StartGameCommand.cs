using GameManager.Application.Contracts.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommand : IRequest<ICommandResponse>
{
    public Guid GameId { get; }

    public StartGameCommand(Guid gameId)
    {
        GameId = gameId;
    }
}