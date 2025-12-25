using Microsoft.AspNetCore.Mvc;
using TicketFlow.Api.Entities;
using TicketFlow.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TicketFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;

    // Inject the database connection
    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // 1. Convert DTO to Entity
        var ticket = new SupportTicket
        {
            ProblemTitle = request.Title,
            ProblemDescription = request.Description,
            Status = "New",
            CreatedAt = DateTime.UtcNow
        };

        // 2. Save to Database
        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        // 3. Return the result (201 Created)
        return CreatedAtAction(nameof(CreateTicket), new { id = ticket.Id }, ticket);
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _context.SupportTickets.ToListAsync();
        return Ok(tickets);
    }
}