using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class UpdatePlayerCommandTests
{
    [Fact]
    public async Task UpdatePlayerCommand_WithValidData_ShouldReturnValidResponse()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .Create();
        var player = fixture.Build<Player>()
            .FromFactory(() => new Player(PlayerName.Of(fixture.Create<string>()), game))
            .Create();
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