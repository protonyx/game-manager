using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.ReorderPlayers;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class ReorderPlayersCommandTests
{
    [Fact]
    public async Task ReorderPlayersCommand_Should_Reorder_Players()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameId = fixture.Create<Guid>();
        var players = new[]
        {
            fixture.Build<Player>().With(p => p.GameId, gameId).Create(),
            fixture.Build<Player>().With(p => p.GameId, gameId).Create(),
            fixture.Build<Player>().With(p => p.GameId, gameId).Create(),
        };
        IEnumerable<Player> reorderedPlayers = Enumerable.Empty<Player>();
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(gameId))
            .ReturnsAsync(players);
        playerRepo.Setup(t => t.UpdatePlayersAsync(It.IsAny<IEnumerable<Player>>()))
            .Callback((IEnumerable<Player> p) => reorderedPlayers = p)
            .Returns(Task.CompletedTask);

        fixture.SetUser(user =>
        {
            user.AddGameId(gameId)
                .AddPlayerId(players[0].Id)
                .AddAdminRole();
        });

        var sut = fixture.Create<ReorderPlayersCommandHandler>();
        var command = new ReorderPlayersCommand(gameId, new[]
        {
            players[1].Id,
            players[0].Id,
            players[2].Id
        });

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        reorderedPlayers.Select(t => t.Id).Should().BeEquivalentTo(command.PlayerIds);
    }
}