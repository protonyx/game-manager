using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Push.Commands.RegisterSubscription;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class RegisterPushSubscriptionCommandTests
{
    [Fact]
    public async Task Handle_NewEndpoint_CreatesSubscription()
    {
        var fixture = TestUtils.GetTestFixture();
        var repository = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var command = CreateCommand();
        repository.Setup(item => item.GetByEndpointAsync(command.Endpoint, CancellationToken.None))
            .ReturnsAsync((PushSubscription?)null);

        var result = await fixture.Create<RegisterPushSubscriptionCommandHandler>()
            .Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Verify(item => item.CreateAsync(
            It.Is<PushSubscription>(subscription =>
                subscription.PlayerId == command.PlayerId &&
                subscription.GameId == command.GameId &&
                subscription.Endpoint == command.Endpoint &&
                subscription.P256dh == command.P256dh &&
                subscription.Auth == command.Auth),
            CancellationToken.None), Times.Once);
        repository.Verify(item => item.DeleteByEndpointAsync(command.Endpoint, CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingEndpoint_RemovesOldMappingBeforeCreatingNewSubscription()
    {
        var fixture = TestUtils.GetTestFixture();
        var repository = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var command = CreateCommand();
        repository.Setup(item => item.GetByEndpointAsync(command.Endpoint, CancellationToken.None))
            .ReturnsAsync(new PushSubscription(
                Guid.NewGuid(), Guid.NewGuid(), command.Endpoint, "old-key", "old-auth"));

        var result = await fixture.Create<RegisterPushSubscriptionCommandHandler>()
            .Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Verify(item => item.DeleteByEndpointAsync(command.Endpoint, CancellationToken.None), Times.Once);
        repository.Verify(item => item.CreateAsync(It.IsAny<PushSubscription>(), CancellationToken.None), Times.Once);
    }

    private static RegisterPushSubscriptionCommand CreateCommand()
        => new(Guid.NewGuid(), Guid.NewGuid(), "https://push.example/subscription", "public-key", "auth-key");
}
