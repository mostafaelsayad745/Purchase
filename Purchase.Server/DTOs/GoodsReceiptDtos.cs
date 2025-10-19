namespace Purchase.Server.DTOs;

public class CreateGoodsReceiptDto
{
    public int PurchaseOrderId { get; set; }
    public int ReceivedQuantity { get; set; }
    public string QualityStatus { get; set; } = string.Empty; // Accepted, Rejected, PartiallyAccepted
    public string? QualityNotes { get; set; }
    public string? ConditionNotes { get; set; }
    public string? ProofOfReceiptDocument { get; set; }
    public int ReceivedById { get; set; }
}

public class UpdateGoodsReceiptDto
{
    public string QualityStatus { get; set; } = string.Empty;
    public string? QualityNotes { get; set; }
    public string? ConditionNotes { get; set; }
    public string? ProofOfReceiptDocument { get; set; }
}

public class GoodsReceiptDto
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int ReceivedQuantity { get; set; }
    public int ExpectedQuantity { get; set; }
    public decimal QuantityVariancePercentage { get; set; }
    public bool IsQuantityAcceptable { get; set; }
    public string QualityStatus { get; set; } = string.Empty;
    public string? QualityNotes { get; set; }
    public string? ConditionNotes { get; set; }
    public string? ProofOfReceiptDocument { get; set; }
    public int? ReceivedById { get; set; }
    public string? ReceivedByNameAr { get; set; }
    public string? ReceivedByNameEn { get; set; }
    public bool StockUpdated { get; set; }
    public DateTime? StockUpdateDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Related data
    public PurchaseOrderDto? PurchaseOrder { get; set; }
    public PaymentDto? Payment { get; set; }
}

public class GoodsReceiptSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public int ReceivedQuantity { get; set; }
    public int ExpectedQuantity { get; set; }
    public string QualityStatus { get; set; } = string.Empty;
    public bool StockUpdated { get; set; }
    public string ToolNameAr { get; set; } = string.Empty;
    public string ToolNameEn { get; set; } = string.Empty;
}
