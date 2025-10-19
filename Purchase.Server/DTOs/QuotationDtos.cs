using System.ComponentModel.DataAnnotations;

namespace Purchase.Server.DTOs;

public class CreateQuotationDto
{
    [Required]
    public int PurchaseRequestId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int QuantityOffered { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int DeliveryTimeDays { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class QuotationDto
{
    public int Id { get; set; }
    public int PurchaseRequestId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierNameAr { get; set; } = string.Empty;
    public string SupplierNameEn { get; set; } = string.Empty;
    public int QuantityOffered { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int DeliveryTimeDays { get; set; }
    public string? Notes { get; set; }
    public DateTime QuotationDate { get; set; }
    public int? Ranking { get; set; }
    public decimal? CustomScore { get; set; }
    public bool IsSelected { get; set; }
    public string? SelectionReason { get; set; }
    public string? RejectionReason { get; set; }
}

public class SelectQuotationDto
{
    [Required]
    public int QuotationId { get; set; }

    [StringLength(500)]
    public string? SelectionReason { get; set; }
}

public class RejectQuotationDto
{
    [Required]
    public int QuotationId { get; set; }

    [Required]
    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty;
}
