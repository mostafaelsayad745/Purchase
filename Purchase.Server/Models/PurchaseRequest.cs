namespace Purchase.Server.Models;

// Step 3: Purchase Request Approval (First Approval)
public class PurchaseRequest
{
    public int Id { get; set; }
    public int ToolRequestId { get; set; }
    public ToolRequest ToolRequest { get; set; } = null!;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "PendingApproval"; // PendingApproval, Approved, Rejected
    public int? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovalNotes { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}
