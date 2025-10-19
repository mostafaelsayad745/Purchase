namespace Purchase.Server.Models;

public class StockItem
{
    public int Id { get; set; }
    public string ToolId { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int CurrentQuantity { get; set; }
    public int MinimumThreshold { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? LastPurchasePrice { get; set; }
    public DateTime? LastRestockDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ToolRequest> ToolRequests { get; set; } = new List<ToolRequest>();
}
