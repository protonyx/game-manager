using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerTrackerUpdated;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : ICommandHandler<UpdatePlayerCommand, PlayerDTO>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IUserContext _userContext;

    private readonly IMediator _mediator;

    public UpdatePlayerCommandHandler(
        IPlayerRepository playerRepository,
        IMapper mapper,
        IUserContext userContext,
        IMediator mediator)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
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
            || !_userContext.User.IsAuthorizedToModifyPlayer(player.Id, player.GameId))
        {
            return GameErrors.PlayerNotAuthorized("update player");
        }

        // Update Name
        if (!string.IsNullOrWhiteSpace(request.Player.Name))
        {
            var playerNameOrError = PlayerName.From(request.Player.Name);

            if (playerNameOrError.IsFailure)
                return GameErrors.PlayerInvalidName(playerNameOrError.Error);
            
            // Check uniqueness
            var isUnique = await _playerRepository.NameIsUniqueAsync(player.GameId, playerNameOrError.Value,
                player.Id, cancellationToken: cancellationToken);

            if (!isUnique)
                return GameErrors.PlayerInvalidName("Name must be unique");

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