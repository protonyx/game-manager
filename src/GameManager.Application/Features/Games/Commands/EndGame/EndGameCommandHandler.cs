using GameManager.Application.Commands;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Common;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommandHandler : IRequestHandler<EndGameCommand, ICommandResponse>
{
    private readonly IGameRepository _gameRepository;
    
    private readonly IMediator _mediator;

    public EndGameCommandHandler(
        IGameRepository gameRepository,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _mediator = mediator;
    }

    public async Task<ICommandResponse> Handle(EndGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return CommandResponses.NotFound();
        }
        
        if (game.State != GameState.InProgress)
        {
            return CommandResponses.Failure("Game is not in progress");
        }
        
        game.Complete();
        
        var updatedGame = await _gameRepository.UpdateAsync(game);

        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);
        
        return CommandResponses.Success();
    }
}