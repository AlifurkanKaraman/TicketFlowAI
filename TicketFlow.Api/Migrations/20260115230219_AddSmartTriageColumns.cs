using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSmartTriageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SupportTickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "SupportTickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "SupportTickets");
        }
    }
}
