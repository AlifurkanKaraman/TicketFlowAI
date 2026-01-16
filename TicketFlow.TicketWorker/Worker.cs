using TicketFlow.TicketWorker.Services;

namespace TicketFlow.TicketWorker;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for new tickets...");

            // Create a "Scope" (like a mini-environment for one unit of work)
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get our nice clean service
                var processor = scope.ServiceProvider.GetRequiredService<ITicketProcessor>();
                
                // Tell it to do the work
                await processor.ProcessNextTicketAsync(stoppingToken);
            }

            // Sleep for 5 seconds
            await Task.Delay(5000, stoppingToken);
        }
    }
}