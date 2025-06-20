using AutoMapper;
using GameManager.Application.Authorization;
using GameManager.Application.Profiles;

namespace GameManager.Tests;

public class BasicTests
{

    [Fact]
    public void Test_AutoMapper_Profiles()
    {
        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DtoProfile>();
        });

        mappingConfig.AssertConfigurationIsValid();
    }

    [Fact]
    public void PlayerIdentityBuilder_WithHostRole_IsHostReturnsTrue()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var principal = PlayerIdentityBuilder.CreatePrincipal(builder =>
        {
            builder.AddGameId(gameId);
            builder.AddPlayerId(playerId);
            builder.AddHostRole();
        });

        // Act
        var isAdmin = principal.IsHostForGame(gameId);

        // Assert
        isAdmin.Should().BeTrue();
    }

    [Fact]
    public void PlayerIdentityBuilder_ForGame_IsAuthReturnsTrue()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var principal = PlayerIdentityBuilder.CreatePrincipal(builder =>
        {
            builder.AddGameId(gameId);
            builder.AddPlayerId(playerId);
        });

        // Act
        var isAuthForGame = principal.IsAuthorizedToViewGame(gameId);
        var isAuthForPlayer = principal.IsAuthorizedToViewPlayer(playerId, gameId);

        // Assert
        isAuthForGame.Should().BeTrue();
        isAuthForPlayer.Should().BeTrue();
    }

    [Fact]
    public void PlayerIdentityBuilder_ForDifferentGame_IsAuthReturnsFalse()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var principal = PlayerIdentityBuilder.CreatePrincipal(builder =>
        {
            builder.AddGameId(gameId);
            builder.AddPlayerId(playerId);
        });

        // Act
        var testGameId = Guid.NewGuid();
        var testPlayerId = Guid.NewGuid();
        var isAuthForGame = principal.IsAuthorizedToViewGame(testGameId);
        var isAuthForPlayer = principal.IsAuthorizedToViewPlayer(testPlayerId, testGameId);

        // Assert
        isAuthForGame.Should().BeFalse();
        isAuthForPlayer.Should().BeFalse();
    }

}