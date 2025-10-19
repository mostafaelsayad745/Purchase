namespace Purchase.Server.Models;

// Step 6: Purchase Order & Document Management
public class PurchaseOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = "Created"; // Created, Received, Inspected, Paid
    public decimal TotalAmount { get; set; }
    public string? PurchaseOrderDocument { get; set; } // File path
    public string? InvoiceDocument { get; set; }
    public string? DeliveryNoteDocument { get; set; }
    public string? SupplierAgreementDocument { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public GoodsReceipt? GoodsReceipt { get; set; }
}
