namespace GameManager.Server.HostedServices;

public abstract class TimedHostedService : IHostedService, IDisposable
{
    private readonly TimeSpan _timerPeriod;
    
    private Timer? _timer;

    private Task? _executeTask;

    private CancellationTokenSource? _stoppingCts;

    protected TimedHostedService(TimeSpan timerPeriod)
    {
        _timerPeriod = timerPeriod;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        _timer = new Timer(DoWork, null, TimeSpan.Zero, _timerPeriod);

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        
        _stoppingCts!.Cancel();

        if (_executeTask is {IsCompleted: false})
        {
            await Task.WhenAny(_executeTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
        }
    }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    private void DoWork(object? sender)
    {
        if (_executeTask is {IsCompleted: false})
        {
            // Don't run again if the last instance is still running
            return;
        }
        
        _executeTask = ExecuteAsync(_stoppingCts!.Token);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}