namespace TicketFlow.TicketWorker.Services;

public interface ITicketProcessor
{
    Task ProcessNextTicketAsync(CancellationToken stoppingToken);
}