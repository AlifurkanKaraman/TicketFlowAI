using TicketFlow.Api;

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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 1. FIND WORK: Get the first ticket that is "New"
                    var ticketToProcess = context.SupportTickets
                        .FirstOrDefault(t => t.Status == "New");

                    if (ticketToProcess != null)
                    {
                        _logger.LogInformation($"🚀 Processing Ticket #{ticketToProcess.Id}: {ticketToProcess.ProblemTitle}");

                        // 2. DO WORK (Simulated AI)
                        // In the next step, Semantic Kernel goes here!
                        await Task.Delay(2000); // Fake "thinking" time

                        ticketToProcess.Status = "Completed";
                        ticketToProcess.ProblemDescription += "\n\n[AI ANSWER]: Have you tried turning it off and on again?";

                        // 3. SAVE WORK
                        context.Update(ticketToProcess);
                        await context.SaveChangesAsync();

                        _logger.LogInformation($"✅ Ticket #{ticketToProcess.Id} Completed!");
                    }
                    else
                    {
                        _logger.LogInformation("💤 No new tickets. Sleeping...");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong in the loop!");
            }

            // Sleep for 5 seconds before checking again
            await Task.Delay(5000, stoppingToken);
        }
    }
}