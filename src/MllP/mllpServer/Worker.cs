namespace mllpServer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMllpServer _mlpServer;

    public Worker(ILogger<Worker> logger, IMllpServer mllpServer)
    {
        _logger = logger;
        mllpServer.Initialize();
        _mlpServer = mllpServer;        
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _mlpServer.WaitingForClientAsync();
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MLLP Server");
        _mlpServer.Stop();
        return base.StopAsync(cancellationToken);
    }
}
