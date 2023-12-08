using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class JoinGameCommandTests
{
    [Fact]
    public async Task JoinGame_InvalidEntryCode_ReturnsFailure()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetGameByEntryCodeAsync(It.IsAny<EntryCode>(), CancellationToken.None))
            .ReturnsAsync(default(Game));
        
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1");
        var handler = fixture.Create<JoinGameCommandHandler>();

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task JoinGame_WithValidGame_CreatesPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var game = new Game(fixture.Create<string>(), new GameOptions());
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetGameByEntryCodeAsync(It.IsAny<EntryCode>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.CreateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync((Player p, CancellationToken ct) => p);
        var playerValidator = new InlineValidator<Player>();
        fixture.Inject<IValidator<Player>>(playerValidator);
        
        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1");
        
        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepo.Verify(t => t.CreateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}