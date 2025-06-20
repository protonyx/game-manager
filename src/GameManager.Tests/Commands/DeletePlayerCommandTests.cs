using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Commands.DeletePlayer;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

using MediatR;

using Moq;

using Xunit;

namespace GameManager.Tests.Commands;

public class DeletePlayerCommandTests
{
    [Fact]
    public async Task DeletePlayerCommand_Should_Delete_Player_Successfully()
    {
        // Arrange
        var fixture = TestUtils.GetTestFixture();
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player1 = new Player(PlayerName.From("Player 1").Value, game);
        player1.SetOrder(1);
        player1.Promote();
        var player2 = new Player(PlayerName.From("Player 2").Value, game);
        player2.SetOrder(2);

        var repoMock = fixture.Freeze<Mock<IPlayerRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(player1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(player1);
        repoMock.Setup(r => r.GetByIdAsync(player2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(player2);
        repoMock.Setup(r => r.UpdateAsync(player2, It.IsAny<CancellationToken>())).ReturnsAsync(player2);
        repoMock.Setup(r => r.GetByGameIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Player> { player1, player2 });
        repoMock.Setup(r => r.UpdateManyAsync(It.IsAny<IEnumerable<Player>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var gameRepoMock = fixture.Freeze<Mock<IGameRepository>>();
        gameRepoMock.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        gameRepoMock.Setup(r => r.UpdateAsync(game, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var turnRepoMock = fixture.Freeze<Mock<ITurnRepository>>();
        turnRepoMock.Setup(r => r.CreateAsync(It.IsAny<Turn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Turn turn, CancellationToken ct) => turn);

        fixture.SetUser(user =>
        {
            user.AddGameId(game.Id)
                .AddPlayerId(player1.Id)
                .AddHostRole();
        });

        var mediatorMock = fixture.Freeze<Mock<IMediator>>();
        mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = fixture.Create<DeletePlayerCommandHandler>();
        var command = new DeletePlayerCommand(player2.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}