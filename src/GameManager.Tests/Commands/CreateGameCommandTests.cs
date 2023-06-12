using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameManager.Application.Data;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class CreateGameCommandTests
{
    [Fact]
    public async Task TestCreateGameCommand_Succeeds()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        fixture.Freeze<ValidationResult>(f => f.OmitAutoProperties());
        var repo = fixture.Freeze<Mock<IGameRepository>>();
        var validator = fixture.Freeze<Mock<IValidator<Game>>>();
        validator.Setup(t => t.ValidateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsUsingFixture(fixture);

        var sut = fixture.Create<CreateGameCommandHandler>();
        var cmd = fixture.Create<CreateGameCommand>();
        
        // Act
        var response = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        response.Game.Should().NotBeNull();
        validator.Verify(t => t.ValidateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(t => t.CreateAsync(It.IsAny<Game>()), Times.Once);
    }
    
    [Fact]
    public async Task TestCreateGameCommand_FailsValidation_ReturnsCorrectResult()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        fixture.Freeze<ValidationResult>(f =>
            f.OmitAutoProperties()
            .Do(t => t.Errors.Add(new ValidationFailure(fixture.Create<string>(), fixture.Create<string>()))));
        var repo = fixture.Freeze<Mock<IGameRepository>>();
        var validator = fixture.Freeze<Mock<IValidator<Game>>>();
        validator.Setup(t => t.ValidateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsUsingFixture(fixture);

        var sut = fixture.Create<CreateGameCommandHandler>();
        var cmd = fixture.Create<CreateGameCommand>();
        
        // Act
        var response = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        response.Game.Should().BeNull();
        validator.Verify(t => t.ValidateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(t => t.CreateAsync(It.IsAny<Game>()), Times.Never);
        response.ValidationResult.IsValid.Should().BeFalse();
    }
}