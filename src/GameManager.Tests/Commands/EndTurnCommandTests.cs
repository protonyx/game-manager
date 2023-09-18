using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Commands;

public class EndTurnCommandTests
{
    [Fact]
    public async Task EndTurnCommand_WithCurrentPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .With(t => t.State, GameState.InProgress)
            .Create();
        var players = fixture.Build<Player>()
            .FromFactory(() => new Player(fixture.Create<PlayerName>(), game))
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        var player2 = players[1];
        player2.SetOrder(2);
        game.CurrentTurn = new CurrentTurnDetails()
        {
            PlayerId = player1.Id
        };

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player2.Id)))
            .ReturnsUsingFixture(fixture.Build<Game>()
                .With(t => t.EntryCode, EntryCode.New(4))
                .With(g => g.Id, game.Id)
                .With(g => g.CurrentTurn, new CurrentTurnDetails()
                {
                    PlayerId = player2.Id
                }));
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<SuccessfulCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player2.Id)), Times.Once);
    }
    
    [Fact]
    public async Task EndTurnCommand_WithNonCurrentAdminPlayer_ShouldAdvanceToNextPlayer()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .With(t => t.State, GameState.InProgress)
            .Create();
        var players = fixture.Build<Player>()
            .FromFactory(() => new Player(fixture.Create<PlayerName>(), game))
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        player1.Promote();
        var player2 = players[1];
        player2.SetOrder(2);
        game.CurrentTurn = new CurrentTurnDetails()
        {
            PlayerId = player2.Id
        };

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        gameRepo.Setup(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player1.Id)))
            .ReturnsUsingFixture(fixture.Build<Game>()
                .With(t => t.EntryCode, EntryCode.New(4))
                .With(g => g.Id, game.Id)
                .With(g => g.CurrentTurn,  new CurrentTurnDetails()
            {
                PlayerId = player1.Id
            }));
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<SuccessfulCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.Is<Game>(g => g.CurrentTurn.PlayerId == player1.Id)), Times.Once);
    }
    
    [Fact]
    public async Task EndTurnCommand_WithNonCurrentPlayer_ShouldNotBeAllowed()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();

        var game = fixture.Build<Game>()
            .With(t => t.EntryCode, EntryCode.New(4))
            .With(t => t.State, GameState.InProgress)
            .Create();
        var players = fixture.Build<Player>()
            .FromFactory(() => new Player(fixture.Create<PlayerName>(), game))
            .CreateMany(2)
            .ToList();
        var player1 = players[0];
        player1.SetOrder(1);
        var player2 = players[1];
        player2.SetOrder(2);
        game.CurrentTurn = new CurrentTurnDetails()
        {
            PlayerId = player1.Id
        };

        var gameRepo = fixture.Freeze<Mock<IGameRepository>>();
        gameRepo.Setup(t => t.GetByIdAsync(game.Id))
            .ReturnsAsync(game);
        var playerRepo = fixture.Freeze<Mock<IPlayerRepository>>();
        playerRepo.Setup(t => t.GetPlayersByGameIdAsync(game.Id))
            .ReturnsAsync(new List<Player>() {player1, player2});
        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player2.Id);
        });

        var sut = fixture.Create<EndTurnCommandHandler>();
        var cmd = new EndTurnCommand(game.Id);

        // Act
        var result = await sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<AuthorizationErrorCommandResponse>();
        gameRepo.Verify(t => t.UpdateAsync(It.IsAny<Game>()), Times.Never());
    }
}