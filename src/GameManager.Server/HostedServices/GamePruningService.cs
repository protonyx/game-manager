using GameManager.Application.Features.Games.Commands.PruneGames;
using MediatR;

namespace GameManager.Server.HostedServices;

public class GamePruningService : TimedHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly IConfiguration _configuration;

    public GamePruningService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        : base(TimeSpan.FromHours(1))
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var retentionDays = _configuration.GetValue<int>("Retention:Days");

        var cmd = new PruneGamesCommand(TimeSpan.FromDays(retentionDays));

        await mediator.Send(cmd, stoppingToken);
    }
}