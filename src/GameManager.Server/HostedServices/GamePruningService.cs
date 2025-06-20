using GameManager.Application.Features.Games.Commands.PruneGames;

namespace GameManager.Server.HostedServices;

public class GamePruningService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ILogger<GamePruningService> _logger;

    private readonly int _retentionDays;

    public GamePruningService(
        IServiceScopeFactory serviceScopeFactory,
        IConfiguration configuration,
        ILogger<GamePruningService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        _retentionDays = configuration.GetValue<int>("Retention:Days");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_retentionDays <= 0)
        {
            return;
        }

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

        var cmd = new PruneGamesCommand(TimeSpan.FromDays(_retentionDays));

        await mediator.Send(cmd, cancellationToken);
    }
}