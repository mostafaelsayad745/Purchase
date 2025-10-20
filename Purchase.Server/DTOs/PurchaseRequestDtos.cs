using System.ComponentModel.DataAnnotations;

namespace Purchase.Server.DTOs;

public class CreatePurchaseRequestDto
{
    public int? ToolRequestId { get; set; }
    
    public string? ToolRequestName { get; set; }
    
    public int? Quantity { get; set; }
    
    public DateTime? RequestDate { get; set; }
    
    public string? Status { get; set; }
    
    public string? ApproverName { get; set; }
    
    public DateTime? ApprovalDate { get; set; }

    public decimal? EstimatedBudget { get; set; }
}

public class ApprovePurchaseRequestDto
{
    [Required]
    public int PurchaseRequestId { get; set; }

    [Required]
    public bool IsApproved { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }
}

public class PurchaseRequestDto
{
    public int Id { get; set; }
    public int ToolRequestId { get; set; }
    public string ToolNameAr { get; set; } = string.Empty;
    public int QuantityNeeded { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ApprovedById { get; set; }
    public string? ApprovedByNameAr { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovalNotes { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? EstimatedBudget { get; set; }
}
