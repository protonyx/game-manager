using FluentValidation.TestHelper;
using GameManager.Application.Data;
using GameManager.Application.Validation;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Validation;

public class PlayerValidatorTests
{
    [Fact]
    public async Task PlayerValidator_WithNonUniqueName_ReturnsValidationError()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsUsingFixture(fixture);
        var repo = fixture.Freeze<Mock<IPlayerRepository>>();
        repo.Setup(t => t.NameIsUniqueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        var sut = fixture.Create<PlayerValidator>();
        var player = fixture.Create<Player>();
        
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

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(Game));

        var sut = fixture.Create<PlayerValidator>();
        var player = fixture.Create<Player>();
        
        // Act
        var result = await sut.TestValidateAsync(player);
        
        // Assert
        result.ShouldHaveValidationErrorFor(t => t.GameId);
    }

}