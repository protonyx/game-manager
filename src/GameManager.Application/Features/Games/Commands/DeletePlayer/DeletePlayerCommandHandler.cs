using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerDeleted;
using GameManager.Application.Services;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, ICommandResponse>
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

    public async Task<ICommandResponse> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        if (player == null)
        {
            return CommandResponses.NotFound();
        }
        
        if (_userContext.User == null || !_userContext.User.IsAuthorizedForGame(player.GameId))
        {
            return CommandResponses.AuthorizationError("Player is not part of this game");
        }
        else if (!_userContext.User.IsAuthorizedForPlayer(player.Id))
        {
            return CommandResponses.AuthorizationError("Not authorized to update this player");
        }
        
        await _playerRepository.DeleteByIdAsync(request.PlayerId);
        
        await _mediator.Publish(new PlayerDeletedNotification(player), cancellationToken);
        
        // If the deleted player was an admin and there are any remaining players, promote the next player to admin
        if (player.IsAdmin)
        {
            var players = await _playerRepository.GetPlayersByGameIdAsync(player.GameId);

            var nextPlayer = players.Where(t => t.Active).MinBy(t => t.Order);

            if (nextPlayer != null)
            {
                await _playerRepository.UpdateAsync(nextPlayer);
                
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

        return CommandResponses.Success();
    }
}