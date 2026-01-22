using System.ComponentModel.DataAnnotations;

namespace TicketFlow.Api.DTOs;

public class CreateTicketRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}