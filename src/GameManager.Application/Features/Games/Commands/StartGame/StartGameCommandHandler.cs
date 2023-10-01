using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommandHandler : IRequestHandler<StartGameCommand, UnitResult<CommandError>>
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

    public async Task<UnitResult<CommandError>> Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return GameErrors.Commands.GameNotFound(request.GameId);
        }
        
        if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return GameErrors.Commands.GameAlreadyInProgress();
        }
        
        if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return GameErrors.Commands.PlayerNotAuthorized("start the game");
        }

        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id);
        var firstPlayer = players.OrderBy(t => t.Order).First();

        game.Start(firstPlayer);
        
        var updatedGame = await _gameRepository.UpdateAsync(game);
        
        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<CommandError>();
    }
}