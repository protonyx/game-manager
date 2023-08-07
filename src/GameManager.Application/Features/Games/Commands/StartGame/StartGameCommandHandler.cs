using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.StartGame;

public class StartGameCommandHandler : IRequestHandler<StartGameCommand, ICommandResponse>
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

    public async Task<ICommandResponse> Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return CommandResponses.NotFound();
        }
        
        if (game.State != GameState.Preparing)
        {
            return CommandResponses.Failure("Game is already in progress");
        }
        else if (!_userContext.User!.IsAdminForGame(game.Id))
        {
            return CommandResponses.AuthorizationError("Only the game creator can start the game");
        }

        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id);
        var firstPlayer = players.OrderBy(t => t.Order).First();

        game.State = GameState.InProgress;
        game.StartedDate = DateTime.UtcNow;
        game.CurrentTurn = new CurrentTurnDetails()
        {
            PlayerId = firstPlayer.Id,
            StartTime = DateTime.UtcNow
        };
        
        var updatedGame = await _gameRepository.UpdateAsync(game);
        
        await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);

        return CommandResponses.Success();
    }
}