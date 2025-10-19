namespace Purchase.Server.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Requester, StockManager, FirstApprover, SecondApprover, FinancialOfficer
    public int? WorkAreaId { get; set; }
    public WorkArea? WorkArea { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ToolRequest> RequestedTools { get; set; } = new List<ToolRequest>();
    public ICollection<PurchaseRequest> ApprovedPurchaseRequests { get; set; } = new List<PurchaseRequest>();
    public ICollection<Quotation> SelectedQuotations { get; set; } = new List<Quotation>();
    public ICollection<Payment> ProcessedPayments { get; set; } = new List<Payment>();
}
