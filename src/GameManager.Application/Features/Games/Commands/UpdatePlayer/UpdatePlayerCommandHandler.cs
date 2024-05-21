using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerTrackerUpdated;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Result<PlayerDTO, ApplicationError>>
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

    public async Task<Result<PlayerDTO, ApplicationError>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (player == null)
        {
            return GameErrors.PlayerNotFound(request.PlayerId);
        }

        if (_userContext.User == null
            || !_userContext.User.IsAuthorizedToViewGame(player.GameId)
            || !_userContext.User.IsAuthorizedToModifyPlayer(player.Id))
        {
            return GameErrors.PlayerNotAuthorized("update player");
        }

        // Update Name
        if (!string.IsNullOrWhiteSpace(request.Player.Name))
        {
            var playerNameOrError = PlayerName.From(request.Player.Name);

            if (playerNameOrError.IsFailure)
                return GameErrors.PlayerInvalidName(playerNameOrError.Error);

            player.SetName(playerNameOrError.Value);
        }

        // Update trackers
        var pendingTrackerNotifications = new List<PlayerTrackerUpdatedNotification>();
        foreach (var tracker in request.Player.TrackerValues)
        {
            var trackerResult = player.SetTracker(tracker.Key, tracker.Value);

            if (trackerResult.IsSuccess)
            {
                pendingTrackerNotifications.Add(new PlayerTrackerUpdatedNotification()
                {
                    PlayerId = player.Id,
                    TrackerId = tracker.Key,
                    NewValue = tracker.Value
                });
            }
        }

        // Validate
        var result = await _playerValidator.ValidateAsync(player, cancellationToken);

        if (!result.IsValid)
        {
            return ApplicationError.Validation<Player>(result);
        }

        var updatedPlayer = await _playerRepository.UpdateAsync(player, cancellationToken);

        await _mediator.Publish(new PlayerUpdatedNotification(updatedPlayer), cancellationToken);

        foreach (var pendingNotification in pendingTrackerNotifications)
        {
            await _mediator.Publish(pendingNotification, cancellationToken);
        }

        var dto = _mapper.Map<PlayerDTO>(updatedPlayer);

        return dto;
    }
}