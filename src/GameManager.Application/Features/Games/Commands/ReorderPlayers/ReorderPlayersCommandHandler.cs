using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;

namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class ReorderPlayersCommandHandler : IRequestHandler<ReorderPlayersCommand, UnitResult<ApplicationError>>
{
    private readonly IGameRepository _gameRepository;
    
    private readonly IPlayerRepository _playerRepository;

    private readonly IMediator _mediator;

    private readonly IUserContext _userContext;

    public ReorderPlayersCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        IMediator mediator,
        IUserContext userContext)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task<UnitResult<ApplicationError>> Handle(ReorderPlayersCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }
        
        if (!_userContext.User!.IsAuthorizedToViewGame(game.Id))
        {
            return GameErrors.PlayerNotAuthorized();
        }

        var players = await _playerRepository.GetPlayersByGameIdAsync(request.GameId, cancellationToken);

        var newIdList = request.PlayerIds;
        var existingIdSet = players.Where(p => p.Active).Select(p => p.Id).ToHashSet();
        
        // Rearrange players based on input list
        var nextOrder = 1;
        foreach (var playerId in newIdList)
        {
            var player = players.FirstOrDefault(p => p.Id == playerId);

            if (player != null)
            {
                player.SetOrder(nextOrder++);
                existingIdSet.Remove(playerId);
            }
        }

        foreach (var playerId in existingIdSet)
        {
            var player = players.FirstOrDefault(p => p.Id == playerId);

            player?.SetOrder(nextOrder++);
        }

        await _playerRepository.UpdateManyAsync(players.ToList(), cancellationToken);

        foreach (var player in players)
        {
            await _mediator.Publish(new PlayerUpdatedNotification(player), cancellationToken);
        }
        
        return UnitResult.Success<ApplicationError>();
    }
}