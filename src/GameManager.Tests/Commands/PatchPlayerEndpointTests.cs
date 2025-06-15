using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using FluentAssertions;

using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

using Moq;

using Xunit;

namespace GameManager.Tests.Commands;

public class PatchPlayerEndpointTests {
    [Fact]
    public async Task PatchPlayer_Should_Update_Player_Properties() {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var game = new Game(GameName.From("Test Game").Value, new GameOptions());
        var player = new Player(PlayerName.From("Old Name").Value, game);
        var playerDto = new PlayerDTO { Id = player.Id, Name = "New Name" };

        var repoMock = fixture.Freeze<Mock<IPlayerRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(player.Id, It.IsAny<CancellationToken>())).ReturnsAsync(player);
        repoMock.Setup(r => r.NameIsUniqueAsync(game.Id, It.IsAny<PlayerName>(), player.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repoMock.Setup(r => r.UpdateAsync(player, It.IsAny<CancellationToken>())).ReturnsAsync(player);

        var mapperMock = fixture.Freeze<Mock<IMapper>>();
        mapperMock.Setup(m => m.Map<PlayerDTO>(player)).Returns(playerDto);

        fixture.SetUser(user => {
            user.AddGameId(game.Id)
                .AddPlayerId(player.Id)
                .AddHostRole();
        });

        var handler = fixture.Create<UpdatePlayerCommandHandler>();
        var command = new UpdatePlayerCommand(player.Id, new PlayerDTO { Name = "New Name" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        repoMock.Verify(r => r.UpdateAsync(player, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PatchPlayer_Should_Return_NotFound_If_Player_Does_Not_Exist() {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var playerId = Guid.NewGuid();
        var repoMock = fixture.Freeze<Mock<IPlayerRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync((Player?)null);

        var handler = fixture.Create<UpdatePlayerCommandHandler>();
        var command = new UpdatePlayerCommand(playerId, new PlayerDTO { Name = "New Name" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.NotFoundError);
    }
}
