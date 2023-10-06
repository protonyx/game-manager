using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommandHandler : IRequestHandler<StartGameCommand, UnitResult<ApplicationError>>
{
    private readonly IGameRepository _gameRepository;
    
    private readonly IPlayerRepository _playerRepository;
    
    private readonly IUserContext _userContext;
    
    private readonly IMediator _mediator;

    public StartGameCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        IUserContext userContext,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _userContext = userContext;
        _mediator = mediator;
    }

    public async Task<UnitResult<ApplicationError>> Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }
        
        if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return GameErrors.GameAlreadyInProgress();
        }
        
        if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return GameErrors.PlayerNotAuthorized("start the game");
        }

        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id, cancellationToken);
        var firstPlayer = players.OrderBy(t => t.Order).First();

        game.Start(firstPlayer);
        
        var updatedGame = await _gameRepository.UpdateAsync(game, cancellationToken);
        
        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}