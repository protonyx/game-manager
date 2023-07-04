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
        var player = fixture.Create<Player>();
        var playerRepository = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepository.Setup(x => x.GetByIdAsync(player.Id))
            .ReturnsAsync(player);
        playerRepository.Setup(x => x.UpdateAsync(It.Is<Player>(p => p.Id == player.Id)))
            .ReturnsAsync((Player p) => p);
        var playerValidator = new InlineValidator<Player>();
        fixture.Inject<IValidator<Player>>(playerValidator);
        fixture.SetUser(user =>
        {
            user.AddGameId(player.GameId)
                .AddPlayerId(player.Id);
        });

        var sut = fixture.Create<UpdatePlayerCommandHandler>();
        var command = new UpdatePlayerCommand()
        {
            PlayerId = player.Id,
            Player = fixture.Build<PlayerDTO>()
                .With(p => p.Id, player.Id)
                .Create()
        };
        
        // Act
        var result = await sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<EntityCommandResponse>();
        playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>()), Times.Once);
    }
}