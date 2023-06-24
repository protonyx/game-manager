using GameManager.Application.Features.Games.Commands.PruneGames;
using MediatR;

namespace GameManager.Server.HostedServices;

public class GamePruningService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly IConfiguration _configuration;

    private readonly ILogger<GamePruningService> _logger;

    public GamePruningService(
        IServiceScopeFactory serviceScopeFactory,
        IConfiguration configuration,
        ILogger<GamePruningService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game Pruning Service running");
        
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        await DoWork(stoppingToken);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Game Pruning Service is stopping");
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var retentionDays = _configuration.GetValue<int>("Retention:Days");

        var cmd = new PruneGamesCommand(TimeSpan.FromDays(retentionDays));

        await mediator.Send(cmd, cancellationToken);
    }
}