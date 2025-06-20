using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games;
using GameManager.Application.Features.Games.Commands.EndGame;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using MediatR;
using Moq;

using Xunit;


namespace GameManager.Tests.Commands;

public class EndGameCommandTests
{
    [Fact]
    public async Task EndGameCommand_Should_End_Game_Successfully()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameName = GameName.From("Test Game");
        var options = new GameOptions();
        var game = new Game(gameName.Value, options);
        game.Start(new Player(PlayerName.From("Host").Value, game));
        var hostId = Guid.NewGuid();
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id);
            user.AddPlayerId(hostId);
            user.AddHostRole();
        });
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        gameRepo.Setup(r => r.UpdateAsync(It.Is<Game>(g => g.State == GameState.Complete), It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var handler = fixture.Create<EndGameCommandHandler>();
        var command = new EndGameCommand(game.Id);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        gameRepo.Verify(r => r.UpdateAsync(It.Is<Game>(g => g.State == GameState.Complete), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EndGameCommand_Should_Fail_If_Not_Host()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameName = GameName.From("Test Game");
        var options = new GameOptions();
        var game = new Game(gameName.Value, options);
        game.Start(new Player(PlayerName.From("Host").Value, game));
        var playerId = Guid.NewGuid();
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id);
            user.AddPlayerId(playerId);
            // No host role
        });
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var handler = fixture.Create<EndGameCommandHandler>();
        var command = new EndGameCommand(game.Id);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.AuthorizationError);
        result.Error.ErrorCode.Should().Be(GameErrors.ErrorCodes.PlayerNotHost);
    }

    [Fact]
    public async Task EndGameCommand_Should_Fail_If_Game_Not_In_Progress()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameName = GameName.From("Test Game");
        var options = new GameOptions();
        var game = new Game(gameName.Value, options); // Not started
        var hostId = Guid.NewGuid();
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id);
            user.AddPlayerId(hostId);
            user.AddHostRole();
        });
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var handler = fixture.Create<EndGameCommandHandler>();
        var command = new EndGameCommand(game.Id);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.ValidationError);
        result.Error.ErrorCode.Should().Be(GameErrors.ErrorCodes.GameInvalidState);
    }

    [Fact]
    public async Task EndGameCommand_Should_Fail_If_Game_Not_Found()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameId = Guid.NewGuid();
        fixture.SetUser(user =>
        {
            user.AddGameId(gameId);
            user.AddPlayerId(Guid.NewGuid());
            user.AddHostRole();
        });
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync((Game) null!);

        var handler = fixture.Create<EndGameCommandHandler>();
        var command = new EndGameCommand(gameId);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.NotFoundError);
    }
}