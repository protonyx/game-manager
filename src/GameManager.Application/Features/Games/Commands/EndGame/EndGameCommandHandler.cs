using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommandHandler : IRequestHandler<EndGameCommand, UnitResult<CommandError>>
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

    public async Task<UnitResult<CommandError>> Handle(EndGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return GameErrors.Commands.GameNotFound(request.GameId);
        }
        
        if (game.State != GameState.InProgress)
        {
            return GameErrors.Commands.GameNotInProgress(game.Id);
        }
        
        game.Complete();
        
        var updatedGame = await _gameRepository.UpdateAsync(game);

        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<CommandError>();
    }
}