using System.ComponentModel.DataAnnotations;

namespace PaperTrader.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}