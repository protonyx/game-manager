using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.ReorderPlayers;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class ReorderPlayersCommandTests
{
    [Fact]
    public async Task ReorderPlayersCommand_Should_Reorder_Players()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());
        var players = fixture.BuildPlayer(game)
            .CreateMany(3)
            .ToList();

        ICollection<Player> reorderedPlayers = null;
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetByGameIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(players);
        playerRepo.Setup(t => t.UpdateManyAsync(It.IsAny<IEnumerable<Player>>(), CancellationToken.None))
            .Callback((IEnumerable<Player> p, CancellationToken ct) => reorderedPlayers = p.ToList())
            .Returns(Task.CompletedTask);

        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(players[0].Id)
                .AddHostRole();
        });

        var sut = fixture.Create<ReorderPlayersCommandHandler>();
        var command = new ReorderPlayersCommand(game.Id, new[]
        {
            players[1].Id,
            players[0].Id,
            players[2].Id
        });

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reorderedPlayers.Should().NotBeNull();
        reorderedPlayers.Select(t => t.Id).Should().BeEquivalentTo(command.PlayerIds);
    }
}