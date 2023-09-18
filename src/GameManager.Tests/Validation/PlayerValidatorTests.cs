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
        
        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .Create();
        var player = fixture.Build<Player>()
            .FromFactory(() => new Player(PlayerName.Of(fixture.Create<string>()), game))
            .Create();

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(game);
        var repo = fixture.Freeze<Mock<IPlayerRepository>>();
        repo.Setup(t => t.NameIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>()))
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
        
        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .Create();
        var player = fixture.Build<Player>()
            .FromFactory(() => new Player(PlayerName.Of(fixture.Create<string>()), game))
            .Create();

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(Game));

        var sut = fixture.Create<PlayerValidator>();
        
        // Act
        var result = await sut.TestValidateAsync(player);
        
        // Assert
        result.ShouldHaveValidationErrorFor(t => t.GameId);
    }

}