namespace TicketFlow.Api.Entities;

public class SupportTicket
{
    public int Id { get; set; }
    public string ProblemTitle { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}