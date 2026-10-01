using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.GameUpdated;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class GameUpdatedPushNotificationHandlerTests
{
    [Fact]
    public async Task Handle_OfflineCurrentPlayer_SendsPushAndRecordsNotification()
    {
        var fixture = TestUtils.GetTestFixture();
        var game = CreateInProgressGame(fixture, out var player);
        var subscription = new PushSubscription(
            player.Id, game.Id, "https://push.example/subscription", "public-key", "auth-key");
        var subscriptions = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var players = fixture.Freeze<Mock<IPlayerRepository>>();
        var sender = fixture.Freeze<Mock<IPushSender>>();
        players.Setup(repository => repository.HasLiveConnectionAsync(
                player.Id, It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(false);
        subscriptions.Setup(repository => repository.GetByPlayerIdAsync(player.Id, CancellationToken.None))
            .ReturnsAsync(new[] { subscription });
        sender.Setup(service => service.SendAsync(
                It.IsAny<PushTarget>(), It.IsAny<PushPayload>(), CancellationToken.None))
            .ReturnsAsync(PushSendResult.Sent);

        await fixture.Create<GameUpdatedPushNotificationHandler>()
            .Handle(new GameUpdatedNotification(game), CancellationToken.None);

        sender.Verify(service => service.SendAsync(
            It.Is<PushTarget>(target => target.Endpoint == subscription.Endpoint),
            It.Is<PushPayload>(payload =>
                payload.Title == "It's your turn" &&
                payload.Body == game.Name.Value &&
                payload.Url == "/game"),
            CancellationToken.None), Times.Once);
        subscriptions.Verify(repository => repository.UpdateAsync(subscription, CancellationToken.None), Times.Once);
        subscription.LastNotifiedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_PlayerHasLiveConnection_DoesNotSendPush()
    {
        var fixture = TestUtils.GetTestFixture();
        var game = CreateInProgressGame(fixture, out var player);
        var players = fixture.Freeze<Mock<IPlayerRepository>>();
        var sender = fixture.Freeze<Mock<IPushSender>>();
        players.Setup(repository => repository.HasLiveConnectionAsync(
                player.Id, It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(true);

        await fixture.Create<GameUpdatedPushNotificationHandler>()
            .Handle(new GameUpdatedNotification(game), CancellationToken.None);

        sender.Verify(service => service.SendAsync(
            It.IsAny<PushTarget>(), It.IsAny<PushPayload>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_GoneEndpoint_DeletesSubscription()
    {
        var fixture = TestUtils.GetTestFixture();
        var game = CreateInProgressGame(fixture, out var player);
        var subscription = new PushSubscription(
            player.Id, game.Id, "https://push.example/subscription", "public-key", "auth-key");
        var subscriptions = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var players = fixture.Freeze<Mock<IPlayerRepository>>();
        var sender = fixture.Freeze<Mock<IPushSender>>();
        players.Setup(repository => repository.HasLiveConnectionAsync(
                player.Id, It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(false);
        subscriptions.Setup(repository => repository.GetByPlayerIdAsync(player.Id, CancellationToken.None))
            .ReturnsAsync(new[] { subscription });
        sender.Setup(service => service.SendAsync(
                It.IsAny<PushTarget>(), It.IsAny<PushPayload>(), CancellationToken.None))
            .ReturnsAsync(PushSendResult.Gone);

        await fixture.Create<GameUpdatedPushNotificationHandler>()
            .Handle(new GameUpdatedNotification(game), CancellationToken.None);

        subscriptions.Verify(repository =>
            repository.DeleteByEndpointAsync(subscription.Endpoint, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_CompletedGame_DeletesAllGameSubscriptions()
    {
        var fixture = TestUtils.GetTestFixture();
        var game = CreateInProgressGame(fixture, out _);
        game.Complete();
        var subscriptions = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var sender = fixture.Freeze<Mock<IPushSender>>();

        await fixture.Create<GameUpdatedPushNotificationHandler>()
            .Handle(new GameUpdatedNotification(game), CancellationToken.None);

        subscriptions.Verify(repository =>
            repository.DeleteByGameIdAsync(game.Id, CancellationToken.None), Times.Once);
        sender.Verify(service => service.SendAsync(
            It.IsAny<PushTarget>(), It.IsAny<PushPayload>(), CancellationToken.None), Times.Never);
    }

    private static Game CreateInProgressGame(IFixture fixture, out Player player)
    {
        var game = new Game(GameName.From("Push test game").Value, new GameOptions());
        player = fixture.BuildPlayer(game).Create();
        player.SetOrder(1);
        game.Start(player);
        return game;
    }
}
