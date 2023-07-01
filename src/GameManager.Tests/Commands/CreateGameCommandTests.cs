using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class CreateGameCommandTests
{
    [Fact]
    public async Task CreateGameCommandHandler_WithValidInput_ShouldCallRepositoryCreate()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var validator = new InlineValidator<Game>();
        fixture.Inject<IValidator<Game>>(validator);

        var repo = fixture.Freeze<Mock<IGameRepository>>();
        repo.Setup(t => t.CreateAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game entity) => entity);

        var sut = fixture.Create<CreateGameCommandHandler>();
        var cmd = fixture.Create<CreateGameCommand>();
        
        // Act
        var response = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<EntityCommandResponse>();
        repo.Verify(t => t.CreateAsync(It.IsAny<Game>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateGameCommandHandler_WithInvalidInput_ReturnsValidationError()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var validator = new InlineValidator<Game>();
        validator.RuleFor(t => t.Id).MustAsync((t, ct) => Task.FromResult(false));
        fixture.Inject<IValidator<Game>>(validator);
        
        var repo = fixture.Freeze<Mock<IGameRepository>>();

        var sut = fixture.Create<CreateGameCommandHandler>();
        var cmd = fixture.Create<CreateGameCommand>();
        
        // Act
        var response = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<ValidationErrorCommandResponse>();
        repo.Verify(t => t.CreateAsync(It.IsAny<Game>()), Times.Never);
        response.As<ValidationErrorCommandResponse>().Result.IsValid.Should().BeFalse();
    }
}