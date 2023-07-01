using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;

namespace GameManager.Tests.Commands;

public class UpdatePlayerCommandTests
{
    [Fact]
    public async Task UpdatePlayerCommand_WithValidData_ShouldReturnValidResponse()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var playerRepository = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(fixture.Create<Player>());
        playerRepository.Setup(x => x.UpdateAsync(It.IsAny<Player>()))
            .Returns(Task.CompletedTask);
        var playerValidator = new InlineValidator<Player>();
        fixture.Inject<IValidator<Player>>(playerValidator);

        var handler = fixture.Create<UpdatePlayerCommandHandler>();
        var command = new UpdatePlayerCommand()
        {
            PlayerId = fixture.Create<Guid>(),
            Player = fixture.Create<PlayerDTO>()
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<EntityCommandResponse>();
        playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>()), Times.Once);
    }
}