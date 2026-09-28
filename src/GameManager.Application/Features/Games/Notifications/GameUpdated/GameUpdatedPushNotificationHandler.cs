using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace GameManager.Application.Features.Games.Notifications.GameUpdated;

public class GameUpdatedPushNotificationHandler(
    IPushSubscriptionRepository subscriptions,
    IPlayerRepository players,
    IPushSender pushSender,
    ILogger<GameUpdatedPushNotificationHandler> logger) : INotificationHandler<GameUpdatedNotification>
{
    private static readonly TimeSpan LiveConnectionWindow = TimeSpan.FromSeconds(90);

    public async Task Handle(GameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var game = notification.Game;

            if (game.State == GameState.Complete)
            {
                await subscriptions.DeleteByGameIdAsync(game.Id, cancellationToken);
                return;
            }

            var nextPlayerId = game.CurrentTurn?.PlayerId;
            if (nextPlayerId is null ||
                await players.HasLiveConnectionAsync(
                    nextPlayerId.Value, DateTime.UtcNow - LiveConnectionWindow, cancellationToken))
            {
                return;
            }

            var playerSubscriptions = await subscriptions.GetByPlayerIdAsync(nextPlayerId.Value, cancellationToken);
            if (playerSubscriptions.Count == 0)
            {
                return;
            }

            var payload = new PushPayload("It's your turn", game.Name.Value, "/game", $"turn-{game.Id}");
            foreach (var subscription in playerSubscriptions)
            {
                var result = await pushSender.SendAsync(
                    new PushTarget(subscription.Endpoint, subscription.P256dh, subscription.Auth),
                    payload,
                    cancellationToken);

                if (result == PushSendResult.Gone)
                {
                    await subscriptions.DeleteByEndpointAsync(subscription.Endpoint, cancellationToken);
                }
                else if (result == PushSendResult.Sent)
                {
                    subscription.MarkNotified();
                    await subscriptions.UpdateAsync(subscription, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed processing push for game update {GameId}", notification.Game.Id);
        }
    }
}
