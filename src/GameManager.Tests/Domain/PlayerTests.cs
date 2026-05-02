using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Domain;

public class PlayerTests
{
    private static Player CreatePlayer()
    {
        var game = new Game(GameName.From("Test").Value, new GameOptions());
        return new Player(PlayerName.From("TestPlayer").Value, game);
    }

    [Fact]
    public void Player_NewPlayer_IsReadyIsFalse()
    {
        var player = CreatePlayer();

        player.IsReady.Should().BeFalse();
    }

    [Fact]
    public void Player_SetReady_SetsIsReadyTrue()
    {
        var player = CreatePlayer();

        player.SetReady();

        player.IsReady.Should().BeTrue();
    }

    [Fact]
    public void Player_ClearReady_SetsIsReadyFalse()
    {
        var player = CreatePlayer();
        player.SetReady();

        player.ClearReady();

        player.IsReady.Should().BeFalse();
    }

    [Fact]
    public void Player_ClearReady_WhenAlreadyFalse_RemainsFlase()
    {
        var player = CreatePlayer();

        player.ClearReady();

        player.IsReady.Should().BeFalse();
    }
}
