using System.ComponentModel.DataAnnotations;

namespace PaperTrader.Models;

public class Portfolio
{
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters.")]
    public string? Name { get; set; }

    public decimal Cash { get; set; } = 0.00m;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}