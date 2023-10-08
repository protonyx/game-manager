using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Domain;

public class GameTests
{
    [Fact]
    public void Game_InPreparingState_CanBeStarted()
    {
        // Arrange
        var game = new Game("Test", new GameOptions());
        var player1 = new Player(PlayerName.From("Player 1").Value, game);
        
        // Act
        var result = game.Start(player1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}