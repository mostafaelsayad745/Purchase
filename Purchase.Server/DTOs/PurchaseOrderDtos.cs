namespace Purchase.Server.DTOs;

public class CreatePurchaseOrderDto
{
    public int QuotationId { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePurchaseOrderDto
{
    public string? PurchaseOrderDocument { get; set; }
    public string? InvoiceDocument { get; set; }
    public string? DeliveryNoteDocument { get; set; }
    public string? SupplierAgreementDocument { get; set; }
    public string? Notes { get; set; }
}

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int QuotationId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PurchaseOrderDocument { get; set; }
    public string? InvoiceDocument { get; set; }
    public string? DeliveryNoteDocument { get; set; }
    public string? SupplierAgreementDocument { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Related data
    public QuotationDto? Quotation { get; set; }
    public GoodsReceiptDto? GoodsReceipt { get; set; }
}

public class PurchaseOrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string SupplierNameAr { get; set; } = string.Empty;
    public string SupplierNameEn { get; set; } = string.Empty;
    public string ToolNameAr { get; set; } = string.Empty;
    public string ToolNameEn { get; set; } = string.Empty;
}
