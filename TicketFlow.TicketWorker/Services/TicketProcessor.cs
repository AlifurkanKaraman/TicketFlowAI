using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using TicketFlow.Api;
using TicketFlow.TicketWorker.Models;

namespace TicketFlow.TicketWorker.Services;

public class TicketProcessor : ITicketProcessor
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TicketProcessor> _logger;
    public TicketProcessor(AppDbContext context, IConfiguration configuration, ILogger<TicketProcessor> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProcessNextTicketAsync(CancellationToken stoppingToken)
    {
        try
        {
            var ticket = _context.SupportTickets.FirstOrDefault(t => t.Status == "New");

            if (ticket == null) return;

            _logger.LogInformation($"🚀 Analyzing Ticket: {ticket.ProblemTitle}");

            // 2. SETUP GEMINI
            var apiKey = _configuration["Gemini:ApiKey"];
            var modelId = _configuration["Gemini:ModelId"];

            // Quick check for configuration
            if (string.IsNullOrEmpty(apiKey)) 
            {
                _logger.LogError("Gemini API Key is missing.");
                return;
            }

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddGoogleAIGeminiChatCompletion(
                modelId: modelId!,
                apiKey: apiKey);
            
            var kernel = kernelBuilder.Build();

            // 3. ASK THE QUESTION
            var prompt = $@"
                You are a Senior IT Support System.
                Analyze this ticket:
                Title: {ticket.ProblemTitle}
                Description: {ticket.ProblemDescription}

                Return ONLY a raw JSON object (no markdown, no ```json tags) with these 3 fields:
                1. 'priority': Choose one [Low, Medium, High, Critical]
                2. 'category': Choose one [Hardware, Software, Network, Access]
                3. 'solution': A polite, helpful response to the user.

                JSON:";

            var result = await kernel.InvokePromptAsync(prompt, cancellationToken: stoppingToken);
            var jsonString = result.ToString().Replace("```json", "").Replace("```", "").Trim();

            try 
            {
                var aiData = JsonSerializer.Deserialize<AiResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (aiData != null)
                {
                    ticket.Priority = aiData.Priority;
                    ticket.Category = aiData.Category;
                    ticket.ProblemDescription += $"\n\n[AI SOLUTION]:\n{aiData.Solution}";
                    ticket.Status = "Completed";

                    _context.Update(ticket);
                    await _context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation($"✅ Done! Priority: {ticket.Priority} | Category: {ticket.Category}");
                }
            }
            catch (JsonException)
            {
                _logger.LogError($"Gemini didn't return valid JSON. It returned: {jsonString}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ticket");
        }
    }
}