using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class JoinGameCommandTests
{
    [Fact]
    public async Task JoinGame_InvalidEntryCode_ReturnsFailure()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetGameByEntryCodeAsync(It.IsAny<EntryCode>()))
            .ReturnsAsync(default(Game));
        
        var cmd = fixture.Build<JoinGameCommand>()
            .With(t => t.EntryCode, EntryCode.New(4).Value)
            .Create();
        var handler = fixture.Create<JoinGameCommandHandler>();

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<FailureCommandResponse>();
        result.As<FailureCommandResponse>().Reason.Should().Be("The entry code is invalid.");
    }

    [Fact]
    public async Task JoinGame_WithValidGame_CreatesPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .Create();
        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetGameByEntryCodeAsync(It.IsAny<EntryCode>()))
           .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.CreateAsync(It.IsAny<Player>()))
          .ReturnsAsync((Player p) => p);
        var playerValidator = new InlineValidator<Player>();
        fixture.Inject<IValidator<Player>>(playerValidator);
        
        var handler = fixture.Create<JoinGameCommandHandler>();
        var cmd = fixture.Build<JoinGameCommand>()
            .With(t => t.EntryCode, EntryCode.New(4).Value)
            .Create();
        
        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<EntityCommandResponse>();
        result.As<EntityCommandResponse>().Value.Should().BeOfType<PlayerCredentialsDTO>();
        playerRepo.Verify(t => t.CreateAsync(It.IsAny<Player>()), Times.Once);
    }
}