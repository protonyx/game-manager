using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommandHandler : IRequestHandler<EndGameCommand, UnitResult<ApplicationError>>
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

    public async Task<UnitResult<ApplicationError>> Handle(EndGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }
        
        if (game.State != GameState.InProgress)
        {
            return GameErrors.GameNotInProgress(game.Id);
        }
        
        game.Complete();
        
        var updatedGame = await _gameRepository.UpdateAsync(game);

        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}