using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.PlayerDeleted;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, ICommandResponse>
{
    private readonly IPlayerRepository _playerRepository;
    
    private readonly IUserContext _userContext;
    
    private readonly IMediator _mediator;

    public DeletePlayerCommandHandler(
        IPlayerRepository playerRepository,
        IUserContext userContext,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _userContext = userContext;
        _mediator = mediator;
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

        return CommandResponses.Success();
    }
}