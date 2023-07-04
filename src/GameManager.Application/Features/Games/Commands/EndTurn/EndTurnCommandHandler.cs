using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : IRequestHandler<EndTurnCommand, ICommandResponse>
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

    public async Task<ICommandResponse> Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var gameId = _userContext.User?.GetGameId();
        var utcNow = DateTime.UtcNow;

        if (!gameId.HasValue)
        {
            return CommandResponses.AuthorizationError();
        }
        
        var game = await _gameRepository.GetByIdAsync(gameId.Value);

        if (game == null)
        {
            return CommandResponses.NotFound();
        }
        
        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id);
        var requestPlayerId = _userContext.User?.GetPlayerId();
        var requestPlayer = players.FirstOrDefault(t => t.Id == requestPlayerId);

        if (requestPlayer == null)
        {
            return CommandResponses.NotFound();
        }
        if (game.CurrentTurnPlayerId == null)
        {
            var firstPlayer = players.First();

            game.CurrentTurnPlayerId = firstPlayer.Id;
        }
        else
        {
            var currentPlayer = players.First(t => t.Id == game.CurrentTurnPlayerId);

            if (requestPlayer != currentPlayer && !requestPlayer.IsAdmin)
            {
                return CommandResponses.AuthorizationError("Only the current player can end the turn");
            }
            
            var nextPlayer = players.FirstOrDefault(t => t.Order > currentPlayer.Order) ?? players.First();

            var turn = new Turn()
            {
                PlayerId = currentPlayer.Id,
                StartTime = game.LastTurnStartTime ?? utcNow,
                EndTime = utcNow
            };
            turn.Duration = turn.EndTime - turn.StartTime;

            await _turnRepository.CreateAsync(turn);

            game.CurrentTurnPlayerId = nextPlayer.Id;
        }

        game.LastTurnStartTime = utcNow;
        var updatedGame = await _gameRepository.UpdateAsync(game);
        
        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return CommandResponses.Success();
    }
}