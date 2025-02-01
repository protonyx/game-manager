using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class EndTurnCommandTests
{
    [Fact]
    public async Task EndTurnCommand_WithCurrentPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        
        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());
        var players = fixture.BuildPlayer(game)
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        var player2 = players[1];
        player2.SetOrder(2);
        game.Start(player1);

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player2.Id), CancellationToken.None))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetByGameIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(new List<Player>() { player1, player2 });
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player2.Id), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task EndTurnCommand_WithNonCurrentAdminPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        
        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());
        var players = fixture.BuildPlayer(game)
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        player1.Promote();
        var player2 = players[1];
        player2.SetOrder(2);
        game.Start(player2);

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player1.Id), CancellationToken.None))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetByGameIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(new List<Player>() { player1, player2 });
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player1.Id), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task EndTurnCommand_WithNonCurrentPlayer_ShouldNotBeAllowed()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        
        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());
        var players = fixture.BuildPlayer(game)
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        var player2 = players[1];
        player2.SetOrder(2);
        game.Start(player1);

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetByGameIdAsync(game.Id, CancellationToken.None))
            .ReturnsAsync(new List<Player>() { player1, player2 });
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player2.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.AuthorizationError);
        gameRepo.Verify(t => t.UpdateAsync(It.IsAny<Game>(), CancellationToken.None), Times.Never());
    }
}