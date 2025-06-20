using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : ICommandHandler<EndTurnCommand>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITurnRepository _turnRepository;

    private readonly IUserContext _userContext;

    private readonly IMediator _mediator;

    public EndTurnCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITurnRepository turnRepository,
        IUserContext userContext,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _turnRepository = turnRepository;
        _userContext = userContext;
        _mediator = mediator;
    }

    public async Task<UnitResult<ApplicationError>> Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }

        var players = await _playerRepository.GetByGameIdAsync(game.Id, cancellationToken);
        var requestPlayerId = _userContext.User?.GetPlayerId();
        var requestPlayer = players.FirstOrDefault(t => t.Id == requestPlayerId);

        if (requestPlayer == null)
        {
            return GameErrors.PlayerNotInGame();
        }

        if (!_userContext.User!.IsAuthorizedToViewGame(game.Id))
        {
            return GameErrors.PlayerNotAuthorized();
        }

        if (game.State != GameState.InProgress)
        {
            return GameErrors.GameNotInProgress(game.Id);
        }

        var currentPlayer = players.First(t => t.Id == game.CurrentTurn.PlayerId);

        if (requestPlayer != currentPlayer && !requestPlayer.IsHost)
        {
            return GameErrors.PlayerNotAuthorized("end the turn");
        }

        var nextPlayer = players
            .Where(t => t.Active)
            .FirstOrDefault(t => t.Order > currentPlayer.Order) ?? players.First();

        var turn = game.SetCurrentTurn(nextPlayer);

        if (turn.IsSuccess)
        {
            await _turnRepository.CreateAsync(turn.Value, cancellationToken);
        }

        var updatedGame = await _gameRepository.UpdateAsync(game, cancellationToken);

        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}