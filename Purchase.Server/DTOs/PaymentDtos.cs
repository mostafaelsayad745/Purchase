namespace Purchase.Server.DTOs;

public class CreatePaymentDto
{
    public int GoodsReceiptId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Bank Transfer, Check, Cash
    public int ProcessedById { get; set; }
    public string? VerificationNotes { get; set; }
}

public class UpdatePaymentDto
{
    public string Status { get; set; } = string.Empty;
    public bool DocumentsVerified { get; set; }
    public bool QuantityVerified { get; set; }
    public bool PriceVerified { get; set; }
    public string? VerificationNotes { get; set; }
}

public class VerifyPaymentDto
{
    public bool DocumentsVerified { get; set; }
    public bool QuantityVerified { get; set; }
    public bool PriceVerified { get; set; }
    public string? VerificationNotes { get; set; }
}

public class ProcessPaymentDto
{
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class PaymentDto
{
    public int Id { get; set; }
    public int GoodsReceiptId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool DocumentsVerified { get; set; }
    public bool QuantityVerified { get; set; }
    public bool PriceVerified { get; set; }
    public int? ProcessedById { get; set; }
    public string? ProcessedByNameAr { get; set; }
    public string? ProcessedByNameEn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? VerificationNotes { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Related data
    public GoodsReceiptDto? GoodsReceipt { get; set; }
}

public class PaymentSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SupplierNameAr { get; set; } = string.Empty;
    public string SupplierNameEn { get; set; } = string.Empty;
}
