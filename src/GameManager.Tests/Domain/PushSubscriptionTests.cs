using GameManager.Domain.Entities;

namespace GameManager.Tests.Domain;

public class PushSubscriptionTests
{
    [Fact]
    public void Constructor_SetsSubscriptionDetails()
    {
        var playerId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var subscription = new PushSubscription(
            playerId, gameId, "https://push.example/subscription", "public-key", "auth-key");

        subscription.Id.Should().NotBeEmpty();
        subscription.PlayerId.Should().Be(playerId);
        subscription.GameId.Should().Be(gameId);
        subscription.Endpoint.Should().Be("https://push.example/subscription");
        subscription.P256dh.Should().Be("public-key");
        subscription.Auth.Should().Be("auth-key");
        subscription.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        subscription.LastNotifiedDate.Should().BeNull();
    }

    [Fact]
    public void MarkNotified_SetsLastNotifiedDate()
    {
        var subscription = new PushSubscription(
            Guid.NewGuid(), Guid.NewGuid(), "https://push.example/subscription", "public-key", "auth-key");

        subscription.MarkNotified();

        subscription.LastNotifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
