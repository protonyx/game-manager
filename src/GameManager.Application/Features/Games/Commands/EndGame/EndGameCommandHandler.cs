using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndGame;

public class EndGameCommandHandler : ICommandHandler<EndGameCommand>
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

        var result = await game.ToResult(GameErrors.GameNotFound(request.GameId))
            .Ensure(g => _userContext.User!.IsAuthorizedToViewGame(g.Id),
                GameErrors.PlayerNotAuthorized())
            .Ensure(g => _userContext.User!.IsHostForGame(g.Id),
                GameErrors.PlayerNotHost())
            .Ensure(g => g.State == GameState.InProgress,
                GameErrors.GameNotInProgress(request.GameId))
            .Tap(async g =>
            {
                g.Complete();

                await _gameRepository.UpdateAsync(g, cancellationToken);
            })
            .Tap(async g =>
            {
                await _mediator.Publish(new GameUpdatedNotification(g), cancellationToken);
            });

        return result.IsSuccess ? UnitResult.Success<ApplicationError>() : UnitResult.Failure(result.Error);
    }
}