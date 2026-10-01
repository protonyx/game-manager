using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.PlayerDeleted;

namespace GameManager.Tests.Commands;

public class PlayerDeletedPushCleanupHandlerTests
{
    [Fact]
    public async Task Handle_DeletesSubscriptionsForDeletedPlayer()
    {
        var fixture = TestUtils.GetTestFixture();
        var repository = fixture.Freeze<Mock<IPushSubscriptionRepository>>();
        var playerId = Guid.NewGuid();
        var notification = new PlayerDeletedNotification(Guid.NewGuid(), playerId);

        await fixture.Create<PlayerDeletedPushCleanupHandler>()
            .Handle(notification, CancellationToken.None);

        repository.Verify(item => item.DeleteByPlayerIdAsync(playerId, CancellationToken.None), Times.Once);
    }
}
