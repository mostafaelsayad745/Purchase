namespace Purchase.Server.Models;

// Step 4: Supplier Quotation & Selection
public class Quotation
{
    public int Id { get; set; }
    public int PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest { get; set; } = null!;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int QuantityOffered { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int DeliveryTimeDays { get; set; }
    public string? Notes { get; set; }
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public int? Ranking { get; set; } // 1, 2, 3 for top 3 offers
    public decimal? CustomScore { get; set; }
    
    // Step 5: Best Offer Approval
    public bool IsSelected { get; set; } = false;
    public int? SelectedById { get; set; }
    public User? SelectedBy { get; set; }
    public DateTime? SelectionDate { get; set; }
    public string? SelectionReason { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder? PurchaseOrder { get; set; }
}
