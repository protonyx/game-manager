// See https://aka.ms/new-console-template for more information

using GameManager.Cli;
using GameManager.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Parsing;

var services = new ServiceCollection();
// services.AddGameManagerClient();

RootCommand rootCommand = new("GameManager CLI");
rootCommand.Options.Add(GlobalOptions.ServerUrlOption);
rootCommand.Options.Add(GlobalOptions.UsernameOption);
rootCommand.Options.Add(GlobalOptions.PasswordOption);

rootCommand.Subcommands.Add(GameCommands.BuildGameCommand(services));

ParseResult parseResult = rootCommand.Parse(args);

if (parseResult.Errors.Count == 0)
{
    await parseResult.InvokeAsync();
    return 0;
}

foreach (ParseError parseError in parseResult.Errors)
{
    Console.Error.WriteLine(parseError.Message);
}
return 1;