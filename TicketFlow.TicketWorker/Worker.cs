using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google; 
using TicketFlow.Api;

namespace TicketFlow.TicketWorker;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
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

                    // 1. FIND WORK
                    var ticket = context.SupportTickets.FirstOrDefault(t => t.Status == "New");

                    if (ticket != null)
                    {
                        _logger.LogInformation($"🚀 Asking Gemini about: {ticket.ProblemTitle}");

                        // 2. SETUP GEMINI (Reads from User Secrets automatically)
                        var apiKey = _configuration["Gemini:ApiKey"];
                        var modelId = _configuration["Gemini:ModelId"];

                        if (string.IsNullOrEmpty(apiKey))
                        {
                            _logger.LogError("CRITICAL: Gemini API Key is missing! Did you run 'dotnet user-secrets set'?");
                            continue;
                        }

                        var kernelBuilder = Kernel.CreateBuilder();
                        kernelBuilder.AddGoogleAIGeminiChatCompletion(
                            modelId: modelId!,
                            apiKey: apiKey);
                        
                        var kernel = kernelBuilder.Build();

                        // 3. ASK THE QUESTION
                        var prompt = $@"
                            You are a helpful IT Support Agent.
                            The user has a problem: {ticket.ProblemTitle}
                            Description: {ticket.ProblemDescription}
                            
                            Please provide a polite, step-by-step solution to fix this.";

                        var result = await kernel.InvokePromptAsync(prompt);

                        // 4. SAVE THE ANSWER
                        ticket.Status = "Completed";
                        ticket.ProblemDescription += $"\n\n[GEMINI SAYS]:\n{result}";

                        context.Update(ticket);
                        await context.SaveChangesAsync();

                        _logger.LogInformation("✅ Gemini successfully answered the ticket!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini had a problem!");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}