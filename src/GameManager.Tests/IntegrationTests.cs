using System.Net.Http.Headers;
using System.Net.Http.Json;
using FastEndpoints.Testing;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using GameManager.Server;
using GameManager.Server.Endpoints.Games;

namespace GameManager.Tests;

[Collection("Integration")]
public class IntegrationTests(GameManagerApp App) : TestBase<GameManagerApp>
{

    [Fact]
    public async Task Test_CreateNewGame()
    {
        // Arrange
        var client = App.CreateClient(new ClientOptions()
        {
            BaseAddress = new("http://localhost/api/v1/")
        });

        var newGame = new CreateGameCommand()
        {
            Name = "Test",
            Options = new GameOptionsDTO()
            {
                ShareOtherPlayerTrackers = true
            },
            Trackers = new List<CreateTrackerDTO>()
            {
                new CreateTrackerDTO()
                {
                    Name = "Score",
                    StartingValue = 0
                }
            }
        };

        // Act
        // Create Game
        var content = new StringContent(JsonConvert.SerializeObject(newGame));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var gameResponse = await client.PostAsync("Games", content, TestContext.Current.CancellationToken);

        gameResponse.EnsureSuccessStatusCode();
        var game = JsonConvert.DeserializeObject<GameDTO>(await gameResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
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
        var playerResponse = await client.PostAsJsonAsync("Games/Join", newPlayer, cancellationToken: TestContext.Current.CancellationToken);

        playerResponse.EnsureSuccessStatusCode();
        var player = JsonConvert.DeserializeObject<PlayerCredentialsDTO>(await playerResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.NotNull(player);
        Assert.True(!string.IsNullOrWhiteSpace(player!.Token));

        // Get game players
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", player.Token);

        var playersResponse = await client.GetAsync($"Games/{game.Id}/Players", TestContext.Current.CancellationToken);
        var players = JsonConvert.DeserializeObject<List<PlayerDTO>>(await playersResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.NotNull(players);
        Assert.Contains(players, p => p.Id == player.PlayerId);
    }
}