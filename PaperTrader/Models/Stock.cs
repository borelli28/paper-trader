using System.ComponentModel.DataAnnotations;

namespace PaperTrader.Models;

public class Stock
{
    public int Id { get; set; }
    [Required]
    public string StockTicker { get; set; } = "None";
    public string Name { get; set; }= "None";
    public decimal SharesTotal { get; set; } = 0.00m;
    public decimal ShareAvgPurchasePrice { get; set; } = 0.00m;
    public decimal ClosePrice { get; set; } = 0.00m;
    public decimal MarketCap { get; set; } = 0.00m;
    public decimal AvgVolume { get; set; } = 0.00m;
    public decimal Earnings { get; set; } = 0.00m;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public int PortfolioId { get; set; }
    public required Portfolio Portfolio { get; set; }
}