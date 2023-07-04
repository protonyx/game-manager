using AutoMapper;
using FluentValidation;
using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, ICommandResponse>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IValidator<Player> _playerValidator;

    private readonly IUserContext _userContext;
    
    private readonly IMediator _mediator;
    
    public UpdatePlayerCommandHandler(
        IPlayerRepository playerRepository,
        IMapper mapper,
        IValidator<Player> playerValidator,
        IUserContext userContext,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
        _playerValidator = playerValidator;
        _userContext = userContext;
        _mediator = mediator;
    }

    public async Task<ICommandResponse> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
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

        // Map changes onto entity
        _mapper.Map(request.Player, player);
        
        // Validate
        var result = await _playerValidator.ValidateAsync(player, cancellationToken);

        if (!result.IsValid)
        {
            return CommandResponses.ValidationError(result);
        }

        var updatedPlayer = await _playerRepository.UpdateAsync(player);
        
        await _mediator.Publish(new PlayerUpdatedNotification(updatedPlayer), cancellationToken);

        var dto = _mapper.Map<PlayerDTO>(updatedPlayer);

        return CommandResponses.Data(request.PlayerId, dto);
    }
}