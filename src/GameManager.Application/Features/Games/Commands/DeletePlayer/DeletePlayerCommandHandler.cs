using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerDeleted;
using GameManager.Application.Services;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, UnitResult<CommandError>>
{
    private readonly IPlayerRepository _playerRepository;
    
    private readonly IUserContext _userContext;
    
    private readonly IMediator _mediator;
    
    private readonly ITokenService _tokenService;

    private readonly IGameClientNotificationService _notificationService;

    public DeletePlayerCommandHandler(
        IPlayerRepository playerRepository,
        IUserContext userContext,
        IMediator mediator,
        ITokenService tokenService,
        IGameClientNotificationService notificationService)
    {
        _playerRepository = playerRepository;
        _userContext = userContext;
        _mediator = mediator;
        _tokenService = tokenService;
        _notificationService = notificationService;
    }

    public async Task<UnitResult<CommandError>> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (player == null)
        {
            return GameErrors.Commands.PlayerNotFound(request.PlayerId);
        }
        
        if (_userContext.User == null 
            || !_userContext.User.IsAuthorizedForGame(player.GameId)
            || !_userContext.User.IsAuthorizedForPlayer(player.Id))
        {
            return CommandError.Authorization("Not authorized to update this player");
        }
        
        player.SoftDelete();
        
        await _playerRepository.UpdateAsync(player, cancellationToken);
        
        await _mediator.Publish(new PlayerDeletedNotification(player), cancellationToken);
        
        // Update player order for remaining players
        var players = await _playerRepository.GetPlayersByGameIdAsync(player.GameId, cancellationToken);
            
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetOrder(i + 1);
        }
        
        // If the deleted player was an admin and there are any remaining players, promote the next player to admin
        if (player.IsAdmin)
        {
            var nextPlayer = players.Where(t => t.Active).MinBy(t => t.JoinedDate);

            if (nextPlayer != null)
            {
                nextPlayer.Promote();
                
                // Generate player token
                var identityBuilder = new PlayerIdentityBuilder();
                identityBuilder.AddGameId(nextPlayer.GameId)
                    .AddPlayerId(nextPlayer.Id);
                identityBuilder.AddAdminRole();
                var token = _tokenService.GenerateToken(identityBuilder.Build());

                var playerCredentials = new PlayerCredentialsDTO
                {
                    GameId = nextPlayer.GameId,
                    PlayerId = nextPlayer.Id,
                    Token = token,
                    IsAdmin = true
                };

                await _notificationService.UpdateCredentials(playerCredentials, cancellationToken);
            }
        }

        await _playerRepository.UpdateManyAsync(players, cancellationToken);

        return UnitResult.Success<CommandError>();
    }
}