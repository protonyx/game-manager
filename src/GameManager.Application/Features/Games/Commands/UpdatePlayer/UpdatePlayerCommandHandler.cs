using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Result<PlayerDTO, CommandError>>
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

    public async Task<Result<PlayerDTO, CommandError>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        if (player == null)
        {
            return GameErrors.Commands.PlayerNotFound(request.PlayerId);
        }
        
        if (_userContext.User == null
            || !_userContext.User.IsAuthorizedForGame(player.GameId)
            || !_userContext.User.IsAuthorizedForPlayer(player.Id))
        {
            return GameErrors.Commands.PlayerNotAuthorized("update player");
        }

        // Map changes onto entity
        _mapper.Map(request.Player, player);
        
        // Validate
        var result = await _playerValidator.ValidateAsync(player, cancellationToken);

        if (!result.IsValid)
        {
            return CommandError.Validation<Player>(result);
        }

        var updatedPlayer = await _playerRepository.UpdateAsync(player);
        
        await _mediator.Publish(new PlayerUpdatedNotification(updatedPlayer), cancellationToken);

        var dto = _mapper.Map<PlayerDTO>(updatedPlayer);

        return dto;
    }
}