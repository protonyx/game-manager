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
        var game = new Game(fixture.Create<string>(), new GameOptions());
        var players = fixture.Build<Player>()
            .FromFactory(() => new Player(PlayerName.Of(fixture.Create<string>()), game))
            .CreateMany(3)
            .ToList();
        ICollection<Player> reorderedPlayers = null;
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(players);
        playerRepo.Setup(t => t.UpdatePlayersAsync(It.IsAny<ICollection<Player>>()))
            .Callback((ICollection<Player> p) => reorderedPlayers = p)
            .Returns(Task.CompletedTask);

        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(players[0].Id)
                .AddAdminRole();
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