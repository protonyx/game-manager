using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayerTurns;
using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;
using Moq;
using AutoMapper;
using Xunit;

namespace GameManager.Tests.Queries;

public class GetPlayerTurnsQueryTests
{
    [Fact]
    public async Task GetPlayerTurnsQuery_Should_Return_Turns()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var turns = new List<Turn>
        {
            new Turn { Id = Guid.NewGuid(), PlayerId = playerId, StartTime = DateTime.UtcNow.AddMinutes(-10), EndTime = DateTime.UtcNow.AddMinutes(-5), Duration = TimeSpan.FromMinutes(5) },
            new Turn { Id = Guid.NewGuid(), PlayerId = playerId, StartTime = DateTime.UtcNow.AddMinutes(-4), EndTime = DateTime.UtcNow, Duration = TimeSpan.FromMinutes(4) }
        };
        var turnDtos = new List<TurnDTO>
        {
            new TurnDTO { StartTime = turns[0].StartTime, EndTime = turns[0].EndTime, DurationSeconds = (int)turns[0].Duration.TotalSeconds },
            new TurnDTO { StartTime = turns[1].StartTime, EndTime = turns[1].EndTime, DurationSeconds = (int)turns[1].Duration.TotalSeconds }
        };

        var repoMock = new Mock<ITurnRepository>();
        repoMock.Setup(r => r.GetTurnsByPlayerId(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(turns);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<TurnDTO>(turns[0])).Returns(turnDtos[0]);
        mapperMock.Setup(m => m.Map<TurnDTO>(turns[1])).Returns(turnDtos[1]);

        var handler = new GetPlayerTurnsQueryHandler(repoMock.Object, mapperMock.Object);
        var query = new GetPlayerTurnsQuery(playerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(turnDtos);
    }
}