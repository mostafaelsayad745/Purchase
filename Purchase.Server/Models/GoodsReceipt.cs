namespace Purchase.Server.Models;

// Step 7: Goods Reception & Stock Management
public class GoodsReceipt
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public int ReceivedQuantity { get; set; }
    public int ExpectedQuantity { get; set; }
    public decimal QuantityVariancePercentage { get; set; }
    public bool IsQuantityAcceptable { get; set; } // Within ±5% tolerance
    public string QualityStatus { get; set; } = string.Empty; // Accepted, Rejected, PartiallyAccepted
    public string? QualityNotes { get; set; }
    public string? ConditionNotes { get; set; }
    public string? ProofOfReceiptDocument { get; set; } // File path
    public int? ReceivedById { get; set; }
    public User? ReceivedBy { get; set; }
    public bool StockUpdated { get; set; } = false;
    public DateTime? StockUpdateDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Payment? Payment { get; set; }
}
