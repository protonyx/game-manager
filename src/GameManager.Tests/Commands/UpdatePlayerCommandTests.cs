using GameManager.Application.Contracts.Persistence;
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
        var dto = fixture.Build<PlayerDTO>()
            .With(p => p.Id, player.Id)
            .With(p => p.Name, "Player 1")
            .Create();
        var command = new UpdatePlayerCommand(player.Id, dto);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}