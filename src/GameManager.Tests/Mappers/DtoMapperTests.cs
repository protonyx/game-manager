using System;
using System.Linq;
using FluentAssertions;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Mappers;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Xunit;

namespace GameManager.Tests.Mappers;

public class DtoMapperTests
{
    private readonly DtoMapper _mapper = new();

    [Fact]
    public void PlayerToDto_Should_Map_State_To_Disconnected_When_No_Connections()
    {
        // Arrange
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("TestPlayer").Value, game);

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.State.Should().Be(PlayerState.Disconnected);
    }

    [Fact]
    public void PlayerToDto_Should_Map_State_To_Connected_When_Has_Connections()
    {
        // Arrange
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("TestPlayer").Value, game);
        player.AddConnection("connection-1");

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.State.Should().Be(PlayerState.Connected);
    }

    [Fact]
    public void PlayerToDto_Should_Map_State_To_Connected_When_Multiple_Connections()
    {
        // Arrange
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("TestPlayer").Value, game);
        player.AddConnection("connection-1");
        player.AddConnection("connection-2");
        player.AddConnection("connection-3");

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.State.Should().Be(PlayerState.Connected);
        player.Connections.Count.Should().Be(3);
    }

    [Fact]
    public void PlayerToDto_Should_Map_TrackerValues_Dictionary_Correctly()
    {
        // Arrange
        var gameOptions = new GameOptions();
        var game = new Game(GameName.From("TestGame").Value, gameOptions);
        
        var tracker1 = Tracker.Create(game, TrackerName.From("Health").Value, 20).Value;
        var tracker2 = Tracker.Create(game, TrackerName.From("Energy").Value, 10).Value;
        var tracker3 = Tracker.Create(game, TrackerName.From("Score").Value, 0).Value;

        // Use reflection to add trackers to the game
        var trackersField = typeof(Game).GetField("_trackers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var trackersList = (System.Collections.Generic.List<Tracker>)trackersField!.GetValue(game)!;
        trackersList.Add(tracker1);
        trackersList.Add(tracker2);
        trackersList.Add(tracker3);

        var player = new Player(PlayerName.From("TestPlayer").Value, game);

        // Verify trackers were initialized with starting values
        player.TrackerValues.Should().HaveCount(3);
        
        // Set different tracker values
        player.SetTracker(tracker1.Id, 15);
        player.SetTracker(tracker2.Id, 25);
        player.SetTracker(tracker3.Id, 100);

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.TrackerValues.Should().NotBeNull();
        dto.TrackerValues.Should().HaveCount(3);
        dto.TrackerValues[tracker1.Id].Should().Be(15);
        dto.TrackerValues[tracker2.Id].Should().Be(25);
        dto.TrackerValues[tracker3.Id].Should().Be(100);
    }

    [Fact]
    public void PlayerToDto_Should_Map_Empty_TrackerValues_To_Empty_Dictionary()
    {
        // Arrange
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("TestPlayer").Value, game);

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.TrackerValues.Should().NotBeNull();
        dto.TrackerValues.Should().BeEmpty();
        dto.TrackerValues.Should().BeOfType<Dictionary<Guid, int>>();
    }

    [Fact]
    public void PlayerToDto_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var game = new Game(GameName.From("TestGame").Value, new GameOptions());
        var player = new Player(PlayerName.From("TestPlayer").Value, game);
        
        // Use reflection to set the Id property
        typeof(Player).GetProperty("Id")!.SetValue(player, playerId);
        
        player.SetColor("blue");
        player.SetReady();
        player.AddConnection("connection-1");

        // Act
        var dto = _mapper.PlayerToDto(player);

        // Assert
        dto.Id.Should().Be(playerId);
        dto.Name.Should().Be("TestPlayer");
        dto.IsHost.Should().BeFalse();
        dto.IsReady.Should().BeTrue();
        dto.Color.Should().Be("blue");
        dto.State.Should().Be(PlayerState.Connected);
    }
}
