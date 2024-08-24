using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Application.Features.Games.Notifications.PlayerDeleted;
using GameManager.Application.Features.Games.Notifications.PlayerPromoted;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : ICommandHandler<DeletePlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IGameRepository _gameRepository;

    private readonly ITurnRepository _turnRepository;

    private readonly IUserContext _userContext;

    private readonly IMediator _mediator;

    public DeletePlayerCommandHandler(
        IPlayerRepository playerRepository,
        IGameRepository gameRepository,
        ITurnRepository turnRepository,
        IUserContext userContext,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _gameRepository = gameRepository;
        _turnRepository = turnRepository;
        _userContext = userContext;
        _mediator = mediator;
    }

    public async Task<UnitResult<ApplicationError>> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var playerToDelete = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (playerToDelete == null)
        {
            return GameErrors.PlayerNotFound(request.PlayerId);
        }

        if (_userContext.User == null
            || !_userContext.User.IsAuthorizedToViewGame(playerToDelete.GameId)
            || !_userContext.User.IsAuthorizedToModifyPlayer(playerToDelete.Id, playerToDelete.GameId))
        {
            return ApplicationError.Authorization("Not authorized to update this player");
        }

        playerToDelete.SoftDelete();

        await _playerRepository.UpdateAsync(playerToDelete, cancellationToken);

        await _mediator.Publish(new PlayerDeletedNotification(playerToDelete), cancellationToken);
        
        var game = await _gameRepository.GetByIdAsync(playerToDelete.GameId, cancellationToken);
        var players = await _playerRepository.GetByGameIdAsync(playerToDelete.GameId, cancellationToken);
        
        // If the player to delete is currently in a turn, advance to the next turn first
        if (game.CheckCurrentTurn(playerToDelete))
        {
            var eligiblePlayers = players
                .Where(t => t.Active && t.Id != playerToDelete.Id)
                .ToArray();
            
            var nextTurn = eligiblePlayers
                               .FirstOrDefault(t => t.Order > playerToDelete.Order)
                           ?? eligiblePlayers.FirstOrDefault();

            if (nextTurn != null)
            {
                // Advance & record turn
                var turn = game.SetCurrentTurn(nextTurn);

                if (turn != null)
                {
                    await _turnRepository.CreateAsync(turn, cancellationToken);
                }
            }
            else
            {
                // No more players, end game
                game.Complete();
            }
            
            var updatedGame = await _gameRepository.UpdateAsync(game, cancellationToken);

            await _mediator.Publish(new GameUpdatedNotification(updatedGame), cancellationToken);
        }
        
        // Update player order for remaining players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetOrder(i + 1);
        }

        // If the deleted player was an admin and there are any remaining players, promote the next player to admin
        if (playerToDelete.IsHost)
        {
            var nextHost = players.Where(t => t.Active).MinBy(t => t.JoinedDate);

            if (nextHost != null)
            {
                nextHost.Promote();
                
                await _mediator.Publish(new PlayerPromotedNotification(nextHost),
                    cancellationToken);
            }
        }

        await _playerRepository.UpdateManyAsync(players, cancellationToken);

        // Notify remaining players of updates to turn order
        foreach (var player in players.Where(p => 
                     p.Id != playerToDelete.Id
                     && p.Active
                     && p.Order >= playerToDelete.Order))
        {
            await _mediator.Publish(new PlayerUpdatedNotification(player), cancellationToken);
        }

        return UnitResult.Success<ApplicationError>();
    }
}