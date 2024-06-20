using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommandHandler : ICommandHandler<StartGameCommand>
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

        var result = await game.ToResult(GameErrors.GameNotFound(request.GameId))
            .Ensure(g => _userContext.User!.IsAuthorizedToViewGame(g.Id),
                GameErrors.PlayerNotAuthorized())
            .Ensure(g => _userContext.User!.IsHostForGame(g.Id),
                GameErrors.PlayerNotHost())
            .Ensure(g => g.State == GameState.Preparing,
                GameErrors.GameAlreadyInProgress())
            .Tap(async g =>
            {
                var players = await _playerRepository.GetPlayersByGameIdAsync(g.Id, cancellationToken);
                var firstPlayer = players.OrderBy(t => t.Order).First();

                g.Start(firstPlayer);
                
                await _gameRepository.UpdateAsync(g, cancellationToken);
            })
            .Tap(async g =>
            {
                await _mediator.Publish(new GameUpdatedNotification(g), cancellationToken);
            });

        return result.IsSuccess ? UnitResult.Success<ApplicationError>() : UnitResult.Failure(result.Error);
    }
}