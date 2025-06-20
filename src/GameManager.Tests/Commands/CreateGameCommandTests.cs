using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
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

        var repo = fixture.Freeze<Mock<IGameRepository>>();
        repo.Setup(t => t.CreateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game entity, CancellationToken ct) => entity);

        var sut = fixture.Create<CreateGameCommandHandler>();
        var cmd = fixture.Create<CreateGameCommand>();

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repo.Verify(t => t.CreateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}