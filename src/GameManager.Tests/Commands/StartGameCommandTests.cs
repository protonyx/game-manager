using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.StartGame;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class StartGameCommandTests
{
    [Fact]
    public async Task StartGameCommand_GameInPreparingState_ShouldStartSuccessfully()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = new Game(fixture.Create<string>(), new GameOptions());
        var players = fixture.BuildPlayer(game)
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        var player2 = players[1];
        player2.SetOrder(2);

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.State == GameState.InProgress), CancellationToken.None))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(new List<Player>() { player1, player2 });
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id)
                .AddHostRole();
        });

        var cmd = new StartGameCommand(game.Id);
        var sut = fixture.Create<StartGameCommandHandler>();

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.State == GameState.InProgress), CancellationToken.None), Times.Once);
    }
}