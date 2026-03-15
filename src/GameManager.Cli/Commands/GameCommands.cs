using GameManager.Client;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.CommandLine;
using System.Net.Http.Headers;

namespace GameManager.Cli.Commands;

public static class GameCommands
{
    public static Command BuildGameCommand(IServiceCollection services)
    {
        Command gameCommand = new("game");

        Command searchCommand = new Command("search", "Search for games");
        searchCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            string serverUrl = parseResult.GetRequiredValue(GlobalOptions.ServerUrlOption);

            services.AddGameManagerClient()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri($"{serverUrl}/graphql");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{parseResult.GetRequiredValue(GlobalOptions.UsernameOption)}:{parseResult.GetRequiredValue(GlobalOptions.PasswordOption)}")));
                });

            using var serviceProvider = services.BuildServiceProvider();
            GameManagerClient gameManagerClient = serviceProvider.GetRequiredService<GameManagerClient>();

            var operationResult = await gameManagerClient.GetGames
                .ExecuteAsync(new GameStateOperationFilterInput()
                {
                    In = [GameState.Preparing, GameState.InProgress]
                }, cancellationToken);

            if (operationResult.Errors.Count > 0)
            {
                foreach (var error in operationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]Error[/]: {error.Message}");
                }
            }

            if (operationResult.Data?.Games?.Edges != null)
            {
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Name");
                table.AddColumn("State");
                table.AddColumn("EntryCode");

                foreach (var edge in operationResult.Data?.Games?.Edges)
                {
                    var game = edge.Node;
                    table.AddRow(game.Id.ToString(), game.Name, game.State.ToString(), game.EntryCode ?? string.Empty);
                }

                AnsiConsole.Write(table);
            }
        });
        gameCommand.Add(searchCommand);

        return gameCommand;
    }
}