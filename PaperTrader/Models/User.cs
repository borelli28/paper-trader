using System.ComponentModel.DataAnnotations;

namespace PaperTrader.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
    public string? Username { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Password must be between 10 and 100 characters")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}