using GameManager.Application.Commands;
using GameManager.Application.Data;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class EndTurnCommandTests
{
    [Fact]
    public async Task EndTurnCommand_WithCurrentPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Create<Game>();
        var player1 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 1)
            .With(t => t.IsAdmin, false)
            .With(t => t.Active, true)
            .Create();
        var player2 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 2)
            .With(t => t.IsAdmin, false)
            .With(t => t.Active, true)
            .Create();
        game.CurrentTurnPlayerId = player1.Id;

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurnPlayerId == player2.Id)))
            .Returns(Task.CompletedTask);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = fixture.Build<EndTurnCommand>()
            .With(t => t.GameId, game.Id)
            .With(t => t.PlayerId, player1.Id)
            .Create();

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<SuccessfulCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurnPlayerId == player2.Id)), Times.Once);
    }
    
    [Fact]
    public async Task EndTurnCommand_WithNonCurrentAdminPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Create<Game>();
        var player1 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 1)
            .With(t => t.IsAdmin, true)
            .With(t => t.Active, true)
            .Create();
        var player2 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 2)
            .With(t => t.IsAdmin, false)
            .With(t => t.Active, true)
            .Create();
        game.CurrentTurnPlayerId = player2.Id;

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurnPlayerId == player1.Id)))
            .Returns(Task.CompletedTask);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = fixture.Build<EndTurnCommand>()
            .With(t => t.GameId, game.Id)
            .With(t => t.PlayerId, player1.Id)
            .Create();

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<SuccessfulCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurnPlayerId == player1.Id)), Times.Once);
    }
    
    [Fact]
    public async Task EndTurnCommand_WithNonCurrentPlayer_ShouldNotBeAllowed()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Create<Game>();
        var player1 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 1)
            .With(t => t.IsAdmin, false)
            .With(t => t.Active, true)
            .Create();
        var player2 = fixture.Build<Player>()
            .With(t => t.GameId, game.Id)
            .With(t => t.Order, 2)
            .With(t => t.IsAdmin, false)
            .With(t => t.Active, true)
            .Create();
        game.CurrentTurnPlayerId = player1.Id;

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = fixture.Build<EndTurnCommand>()
            .With(t => t.GameId, game.Id)
            .With(t => t.PlayerId, player2.Id)
            .Create();

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<AuthorizationErrorCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.IsAny<Game>()), Times.Never());
    }
}