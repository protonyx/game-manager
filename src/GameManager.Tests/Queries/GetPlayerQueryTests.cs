using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayer;
using GameManager.Application.Features.Games;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Mappers;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Moq;
using Xunit;

namespace GameManager.Tests.Queries;

public class GetPlayerQueryTests
{
    private readonly DtoMapper _mapper = new();

    [Fact]
    public async Task GetPlayerQuery_Should_Return_Player()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("Test Player").Value, game);
        typeof(Player).GetProperty("Id")!.SetValue(player, playerId);

        var repoMock = new Mock<IPlayerRepository>();
        repoMock.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(player);

        var handler = new GetPlayerQueryHandler(repoMock.Object, _mapper);
        var query = new GetPlayerQuery(playerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(playerId);
        result.Value.Name.Should().Be("Test Player");
    }

    [Fact]
    public async Task GetPlayerQuery_Should_Return_NotFound_When_Player_Does_Not_Exist()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var repoMock = new Mock<IPlayerRepository>();
        repoMock.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync((Player?) null);
        var handler = new GetPlayerQueryHandler(repoMock.Object, _mapper);
        var query = new GetPlayerQuery(playerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.ErrorType.Should().Be(ApplicationErrorType.NotFoundError);
    }
}