using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games;
using GameManager.Domain.Constants;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class JoinGameCommandTests
{
    private static (IFixture fixture, Mock<IGameRepository> gameRepo, Mock<IPlayerRepository> playerRepo, Game game)
        SetupValidGame()
    {
        var fixture = TestUtils.GetTestFixture();
        var gameName = GameName.From(fixture.Create<string>());
        var game = new Game(gameName.Value, new GameOptions());

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetIdByEntryCodeAsync(It.IsAny<EntryCode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(game.Id);
        gameRepo.Setup(t => t.GetByIdAsync(game.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.CreateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);
        playerRepo.Setup(t => t.NameIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<PlayerName>(), It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        playerRepo.Setup(t => t.GetTakenColorsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        playerRepo.Setup(t => t.ColorIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return (fixture, gameRepo, playerRepo, game);
    }

    [Fact]
    public async Task JoinGame_InvalidEntryCode_ReturnsFailure()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetIdByEntryCodeAsync(It.IsAny<EntryCode>(), CancellationToken.None))
            .ReturnsAsync(default(Guid?));

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
        var (fixture, _, playerRepo, _) = SetupValidGame();

        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1");

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepo.Verify(t => t.CreateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinGame_WithNoColorSpecified_AutoAssignsFirstAvailableColor()
    {
        // Arrange
        var (fixture, _, playerRepo, _) = SetupValidGame();
        var takenColors = new List<string> { PlayerColors.All[0] };
        playerRepo.Setup(t => t.GetTakenColorsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(takenColors);

        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1");

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepo.Verify(t => t.CreateAsync(
            It.Is<Player>(p => p.Color == PlayerColors.All[1]),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinGame_WithValidColor_AssignsSpecifiedColor()
    {
        // Arrange
        var (fixture, _, playerRepo, _) = SetupValidGame();
        var chosenColor = PlayerColors.All[3];

        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1", color: chosenColor);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playerRepo.Verify(t => t.CreateAsync(
            It.Is<Player>(p => p.Color == chosenColor),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinGame_WithColorNotInPalette_ReturnsError()
    {
        // Arrange
        var (fixture, _, _, _) = SetupValidGame();

        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1", color: "#INVALID");

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(GameErrors.ErrorCodes.PlayerInvalidColor);
    }

    [Fact]
    public async Task JoinGame_WithTakenColor_ReturnsError()
    {
        // Arrange
        var (fixture, _, playerRepo, _) = SetupValidGame();
        var takenColor = PlayerColors.All[0];
        playerRepo.Setup(t => t.ColorIsUniqueAsync(It.IsAny<Guid>(), takenColor, It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = new JoinGameCommand(EntryCode.New(4).Value, "Player 1", color: takenColor);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(GameErrors.ErrorCodes.PlayerInvalidColor);
    }
}