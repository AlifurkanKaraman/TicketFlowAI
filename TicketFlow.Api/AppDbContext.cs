using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace TicketFlow.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<KnowledgeBaseItem> KnowledgeBase { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This enables the AI vector math in Postgres
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<KnowledgeBaseItem>(entity =>
        {
            entity.Property(e => e.Embedding)
                .HasColumnType("vector(1536)"); // Matches OpenAI embedding size
        });
    }
}

// === The Tables ===

public class KnowledgeBaseItem
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Vector? Embedding { get; set; } // The "Brain" part
}

public class SupportTicket
{
    public int Id { get; set; }
    public string ProblemTitle { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}