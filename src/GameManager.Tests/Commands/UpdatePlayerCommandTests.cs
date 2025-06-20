using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class UpdatePlayerCommandTests
{
    [Fact]
    public async Task UpdatePlayerCommand_WithValidData_ShouldReturnValidResponse()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());
        var player = fixture.BuildPlayer(game).Create();
        var playerRepository = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepository.Setup(x => x.GetByIdAsync(player.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);
        playerRepository.Setup(x => x.UpdateAsync(It.Is<Player>(p => p.Id == player.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);
        playerRepository.Setup(t => t.NameIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<PlayerName>(), It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.SetUser(user =>
        {
            user.AddGameId(player.GameId)
                .AddPlayerId(player.Id);
        });

        var sut = fixture.Create<UpdatePlayerCommandHandler>();
        var dto = fixture.Build<UpdatePlayerDTO>()
            .With(p => p.Name, "Player 1")
            .Create();
        var command = new UpdatePlayerCommand(player.Id, dto);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlayerCommand_Should_Return_NotFound_If_Player_Does_Not_Exist()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var playerId = Guid.NewGuid();
        var repoMock = fixture.Freeze<Mock<IPlayerRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync((Player?) null);

        var handler = fixture.Create<UpdatePlayerCommandHandler>();
        var command = new UpdatePlayerCommand(playerId, new UpdatePlayerDTO { Name = "New Name" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.NotFoundError);
    }
}