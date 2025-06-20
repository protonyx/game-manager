using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerTrackerUpdated;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler(
    IPlayerRepository playerRepository,
    IGameRepository gameRepository,
    IMapper mapper,
    IUserContext userContext,
    IMediator mediator)
    : ICommandHandler<UpdatePlayerCommand, PlayerDTO>
{
    public async Task<Result<PlayerDTO, ApplicationError>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (player == null)
        {
            return GameErrors.PlayerNotFound(request.PlayerId);
        }

        if (userContext.User == null
            || !userContext.User.IsAuthorizedToViewGame(player.GameId)
            || !userContext.User.IsAuthorizedToModifyPlayer(player.Id, player.GameId))
        {
            return GameErrors.PlayerNotAuthorized("update player");
        }

        var game = await gameRepository.GetByIdAsync(player.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(player.GameId);
        }

        // Update Name
        if (!string.IsNullOrWhiteSpace(request.Player.Name))
        {
            var playerNameOrError = PlayerName.From(request.Player.Name);

            if (playerNameOrError.IsFailure)
            {
                return GameErrors.PlayerInvalidName(playerNameOrError.Error);
            }

            // Check uniqueness
            var isUnique = await playerRepository.NameIsUniqueAsync(player.GameId, playerNameOrError.Value,
                player.Id, cancellationToken: cancellationToken);

            if (!isUnique)
            {
                return GameErrors.PlayerInvalidName("Name must be unique");
            }

            player.SetName(playerNameOrError.Value);
        }

        // Update trackers
        var pendingTrackerNotifications = new List<PlayerTrackerUpdatedNotification>();
        foreach (var tracker in request.Player.TrackerValues)
        {
            // Validate that the input is a valid tracker ID
            if (game.Trackers.Any(t => t.Id.Equals(tracker.Key)))
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
        }

        var updatedPlayer = await playerRepository.UpdateAsync(player, cancellationToken);

        await mediator.Publish(new PlayerUpdatedNotification(updatedPlayer), cancellationToken);

        foreach (var pendingNotification in pendingTrackerNotifications)
        {
            await mediator.Publish(pendingNotification, cancellationToken);
        }

        var dto = mapper.Map<PlayerDTO>(updatedPlayer);

        return dto;
    }
}