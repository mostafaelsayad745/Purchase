namespace Purchase.Server.Models;

// Step 1: Initial Request
public class ToolRequest
{
    public int Id { get; set; }
    public int ToolId { get; set; }
    public StockItem Tool { get; set; } = null!;
    public int QuantityNeeded { get; set; }
    public int WorkAreaId { get; set; }
    public WorkArea WorkArea { get; set; } = null!;
    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string ReasonAr { get; set; } = string.Empty;
    public string ReasonEn { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, InStock, OutOfStock, Rejected
    public bool IsInStock { get; set; }
    public int? QuantityFulfilled { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public string? StockVerificationNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseRequest? PurchaseRequest { get; set; }
}
