using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayerTurns;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Mappers;
using GameManager.Domain.Entities;
using Moq;
using Xunit;

namespace GameManager.Tests.Queries;

public class GetPlayerTurnsQueryTests
{
    private readonly DtoMapper _mapper = new();

    [Fact]
    public async Task GetPlayerTurnsQuery_Should_Return_Turns()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var turns = new List<Turn>
        {
            Turn.Create(playerId, DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(-5)).Value,
            Turn.Create(playerId, DateTime.UtcNow.AddMinutes(-4), DateTime.UtcNow).Value,
        };

        var repoMock = new Mock<ITurnRepository>();
        repoMock.Setup(r => r.GetTurnsByPlayerId(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(turns);

        var handler = new GetPlayerTurnsQueryHandler(repoMock.Object, _mapper);
        var query = new GetPlayerTurnsQuery(playerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].StartTime.Should().Be(turns[0].StartTime);
        result.Value[0].DurationSeconds.Should().Be((int)turns[0].Duration.TotalSeconds);
        result.Value[1].StartTime.Should().Be(turns[1].StartTime);
    }
}