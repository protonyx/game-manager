using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.Notifications.GameUpdated;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : IRequestHandler<EndTurnCommand, UnitResult<CommandError>>
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

    public async Task<UnitResult<CommandError>> Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return GameErrors.Commands.GameNotFound(request.GameId);
        }
        
        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id);
        var requestPlayerId = _userContext.User?.GetPlayerId();
        var requestPlayer = players.FirstOrDefault(t => t.Id == requestPlayerId);

        if (requestPlayer == null)
        {
            return GameErrors.Commands.PlayerNotInGame();
        }

        if (!_userContext.User!.IsAuthorizedForGame(game.Id))
        {
            return GameErrors.Commands.PlayerNotAuthorized();
        }

        if (game.State != GameState.InProgress)
        {
            return GameErrors.Commands.GameNotInProgress(game.Id);
        }
        
        var currentPlayer = players.First(t => t.Id == game.CurrentTurn.PlayerId);

        if (requestPlayer != currentPlayer && !requestPlayer.IsAdmin)
        {
            return GameErrors.Commands.PlayerNotAuthorized("end the turn");
        }

        var nextPlayer = players.FirstOrDefault(t => t.Order > currentPlayer.Order) ?? players.First();

        var turn = new Turn()
        {
            PlayerId = currentPlayer.Id,
            StartTime = game.CurrentTurn?.StartTime ?? utcNow,
            EndTime = utcNow
        };
        turn.Duration = turn.EndTime - turn.StartTime;

        await _turnRepository.CreateAsync(turn);

        game.SetCurrentTurn(nextPlayer);
        
        var updatedGame = await _gameRepository.UpdateAsync(game);
        
        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return UnitResult.Success<CommandError>();
    }
}