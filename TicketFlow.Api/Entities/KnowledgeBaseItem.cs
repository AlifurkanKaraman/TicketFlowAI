using Pgvector;
namespace TicketFlow.Api.Entities;

public class KnowledgeBaseItem
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
}