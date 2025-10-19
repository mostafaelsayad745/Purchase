using Microsoft.EntityFrameworkCore;
using Purchase.Server.Models;

namespace Purchase.Server.Data;

public class PurchaseDbContext : DbContext
{
    public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
    {
    }

    public DbSet<WorkArea> WorkAreas { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<ToolRequest> ToolRequests { get; set; }
    public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // WorkArea Configuration
        modelBuilder.Entity<WorkArea>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(e => e.WorkArea)
                .WithMany()
                .HasForeignKey(e => e.WorkAreaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // StockItem Configuration
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ToolId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LastPurchasePrice).HasPrecision(18, 2);
            entity.HasIndex(e => e.ToolId).IsUnique();
        });

        // ToolRequest Configuration
        modelBuilder.Entity<ToolRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Tool)
                .WithMany(t => t.ToolRequests)
                .HasForeignKey(e => e.ToolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.WorkArea)
                .WithMany(w => w.ToolRequests)
                .HasForeignKey(e => e.WorkAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Requester)
                .WithMany(u => u.RequestedTools)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PurchaseRequest Configuration
        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EstimatedBudget).HasPrecision(18, 2);

            entity.HasOne(e => e.ToolRequest)
                .WithOne(t => t.PurchaseRequest)
                .HasForeignKey<PurchaseRequest>(e => e.ToolRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ApprovedBy)
                .WithMany(u => u.ApprovedPurchaseRequests)
                .HasForeignKey(e => e.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Supplier Configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
        });

        // Quotation Configuration
        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.CustomScore).HasPrecision(10, 2);

            entity.HasOne(e => e.PurchaseRequest)
                .WithMany(p => p.Quotations)
                .HasForeignKey(e => e.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Quotations)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SelectedBy)
                .WithMany(u => u.SelectedQuotations)
                .HasForeignKey(e => e.SelectedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PurchaseOrder Configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.HasOne(e => e.Quotation)
                .WithOne(q => q.PurchaseOrder)
                .HasForeignKey<PurchaseOrder>(e => e.QuotationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GoodsReceipt Configuration
        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityVariancePercentage).HasPrecision(5, 2);

            entity.HasOne(e => e.PurchaseOrder)
                .WithOne(p => p.GoodsReceipt)
                .HasForeignKey<GoodsReceipt>(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReceivedBy)
                .WithMany()
                .HasForeignKey(e => e.ReceivedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.TransactionReference).IsRequired().HasMaxLength(200);

            entity.HasOne(e => e.GoodsReceipt)
                .WithOne(g => g.Payment)
                .HasForeignKey<Payment>(e => e.GoodsReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProcessedBy)
                .WithMany(u => u.ProcessedPayments)
                .HasForeignKey(e => e.ProcessedById)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
