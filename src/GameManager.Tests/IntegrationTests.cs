using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using GameManager.Server;

namespace GameManager.Tests;

[Collection("Integration")]
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Test_CreateNewGame()
    {
        // Arrange
        var client = _factory.CreateClient();

        var newGame = new CreateGameDTO()
        {
            Name = "Test",
            Options = new GameOptionsDTO()
            {
                ShareOtherPlayerTrackers = true
            },
            Trackers = new List<TrackerDTO>()
            {
                new TrackerDTO()
                {
                    Name = "Score",
                    StartingValue = 0
                }
            }
        };
        
        // Act
        var content = new StringContent(JsonConvert.SerializeObject(newGame));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var gameResponse = await client.PostAsync("api/Games", content);

        // Assert
        gameResponse.EnsureSuccessStatusCode();
        var game = JsonConvert.DeserializeObject<GameDTO>(await gameResponse.Content.ReadAsStringAsync());
        Assert.True(game != null);
        Assert.True(game!.EntryCode.Length == 4);
        
        // Join the new game
        var newPlayer = new JoinGameDTO()
        {
            EntryCode = game.EntryCode,
            Name = "Test Player"
        };

        //var content2 = new StringContent(JsonConvert.SerializeObject(newPlayer));
        //content2.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var playerResponse = await client.PostAsJsonAsync("api/Games/Join", newPlayer);

        playerResponse.EnsureSuccessStatusCode();
        var player = JsonConvert.DeserializeObject<PlayerCredentialsDTO>(await playerResponse.Content.ReadAsStringAsync());

        Assert.True(player != null);
        Assert.True(!string.IsNullOrWhiteSpace(player!.Token));
    }
}