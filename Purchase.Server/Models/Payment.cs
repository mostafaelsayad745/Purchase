namespace Purchase.Server.Models;

// Step 8: Payment Processing
public class Payment
{
    public int Id { get; set; }
    public int GoodsReceiptId { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Bank Transfer, Check, Cash
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
    public bool DocumentsVerified { get; set; } = false;
    public bool QuantityVerified { get; set; } = false;
    public bool PriceVerified { get; set; } = false;
    public int? ProcessedById { get; set; }
    public User? ProcessedBy { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? VerificationNotes { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchiveDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
