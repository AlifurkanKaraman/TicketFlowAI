using Microsoft.EntityFrameworkCore;
using TicketFlow.Api.Entities;

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

