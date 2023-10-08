using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommandHandler : IRequestHandler<EndGameCommand, UnitResult<ApplicationError>>
{
    private readonly IGameRepository _gameRepository;

    private readonly IUserContext _userContext;
    
    private readonly IMediator _mediator;

    public EndGameCommandHandler(
        IGameRepository gameRepository,
        IUserContext userContext,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _userContext = userContext;
        _mediator = mediator;
    }

    public async Task<UnitResult<ApplicationError>> Handle(EndGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }
        
        if (!_userContext.User!.IsAuthorizedForGame(game.Id))
        {
            return GameErrors.PlayerNotAuthorized();
        }
        
        if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return GameErrors.PlayerNotAuthorized("start the game");
        }
        
        if (game.State != GameState.InProgress)
        {
            return GameErrors.GameNotInProgress(game.Id);
        }
        
        var result = game.Complete();

        if (result.IsFailure)
        {
            return ApplicationError.Failure(result.Error);
        }
        
        var updatedGame = await _gameRepository.UpdateAsync(game, cancellationToken);

        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}