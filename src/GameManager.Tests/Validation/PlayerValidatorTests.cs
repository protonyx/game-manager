using FluentValidation.TestHelper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Validation;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Validation;

public class PlayerValidatorTests
{
    [Fact]
    public async Task PlayerValidator_WithNonUniqueName_ReturnsValidationError()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = new Game(fixture.Create<string>(), new GameOptions());
        var player = fixture.Build<Player>()
            .FromFactory(() =>
            {
                var playerName = PlayerName.From(fixture.Create<string>().Substring(0, 10));
                return new Player(playerName.Value, game);
            })
            .Create();

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.ExistsAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);
        var repo = fixture.Freeze<Mock<IPlayerRepository>>();
        repo.Setup(t => t.NameIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), CancellationToken.None))
            .ReturnsAsync(false);

        var sut = fixture.Create<PlayerValidator>();

        // Act
        var result = await sut.TestValidateAsync(player);

        // Assert
        result.ShouldHaveValidationErrorFor(t => t.Name);
    }

    [Fact]
    public async Task PlayerValidator_WithInvalidGameId_ReturnsValidationError()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = new Game(fixture.Create<string>(), new GameOptions());
        var player = fixture.Build<Player>()
            .FromFactory(() =>
            {
                var playerName = PlayerName.From(fixture.Create<string>().Substring(0, 10));
                return new Player(playerName.Value, game);
            })
            .Create();

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(default(Game));

        var sut = fixture.Create<PlayerValidator>();

        // Act
        var result = await sut.TestValidateAsync(player);

        // Assert
        result.ShouldHaveValidationErrorFor(t => t.GameId);
    }

}