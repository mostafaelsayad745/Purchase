using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(PurchaseDbContext context, ILogger<PaymentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentSummaryDto>>> GetPayments()
    {
        var payments = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Quotation)
                        .ThenInclude(q => q.Supplier)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentSummaryDto
            {
                Id = p.Id,
                OrderNumber = p.GoodsReceipt.PurchaseOrder.OrderNumber,
                TransactionReference = p.TransactionReference,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                SupplierNameAr = p.GoodsReceipt.PurchaseOrder.Quotation.Supplier.NameAr,
                SupplierNameEn = p.GoodsReceipt.PurchaseOrder.Quotation.Supplier.NameEn
            })
            .ToListAsync();

        return Ok(payments);
    }

    // GET: api/payments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Quotation)
                        .ThenInclude(q => q.Supplier)
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Quotation)
                        .ThenInclude(q => q.PurchaseRequest)
                            .ThenInclude(pr => pr.ToolRequest)
                                .ThenInclude(tr => tr.Tool)
            .Include(p => p.ProcessedBy)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        var dto = MapToDto(payment);
        return Ok(dto);
    }

    // POST: api/payments
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto dto)
    {
        // Validate goods receipt exists
        var goodsReceipt = await _context.GoodsReceipts
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
            .FirstOrDefaultAsync(gr => gr.Id == dto.GoodsReceiptId);

        if (goodsReceipt == null)
        {
            return NotFound("Goods receipt not found");
        }

        if (goodsReceipt.PurchaseOrder.Status != "Inspected")
        {
            return BadRequest("Payment can only be created for inspected goods");
        }

        if (goodsReceipt.QualityStatus != "Accepted")
        {
            return BadRequest("Payment can only be created for accepted goods");
        }

        if (!goodsReceipt.StockUpdated)
        {
            return BadRequest("Payment can only be created after stock has been updated");
        }

        // Check if payment already exists
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(p => p.GoodsReceiptId == dto.GoodsReceiptId);

        if (existingPayment != null)
        {
            return BadRequest("Payment already exists for this goods receipt");
        }

        // Generate transaction reference
        var transactionRef = await GenerateTransactionReference();

        var payment = new Payment
        {
            GoodsReceiptId = dto.GoodsReceiptId,
            TransactionReference = transactionRef,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            Status = "Pending",
            DocumentsVerified = false,
            QuantityVerified = false,
            PriceVerified = false,
            ProcessedById = dto.ProcessedById,
            VerificationNotes = dto.VerificationNotes,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Load relationships for response
        await _context.Entry(payment)
            .Reference(p => p.GoodsReceipt)
            .LoadAsync();
        await _context.Entry(payment.GoodsReceipt)
            .Reference(gr => gr.PurchaseOrder)
            .LoadAsync();

        var result = MapToDto(payment);
        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, result);
    }

    // POST: api/payments/{id}/verify
    [HttpPost("{id}/verify")]
    public async Task<ActionResult> VerifyPayment(int id, VerifyPaymentDto dto)
    {
        var payment = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        if (payment.Status != "Pending")
        {
            return BadRequest("Only pending payments can be verified");
        }

        payment.DocumentsVerified = dto.DocumentsVerified;
        payment.QuantityVerified = dto.QuantityVerified;
        payment.PriceVerified = dto.PriceVerified;
        payment.VerificationNotes = dto.VerificationNotes;
        payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Payment verification updated",
            allVerified = dto.DocumentsVerified && dto.QuantityVerified && dto.PriceVerified
        });
    }

    // POST: api/payments/{id}/process
    [HttpPost("{id}/process")]
    public async Task<ActionResult> ProcessPayment(int id, ProcessPaymentDto dto)
    {
        var payment = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        if (payment.Status != "Pending")
        {
            return BadRequest("Only pending payments can be processed");
        }

        // Check verification
        if (!payment.DocumentsVerified || !payment.QuantityVerified || !payment.PriceVerified)
        {
            return BadRequest("All verification checks must be completed before processing payment");
        }

        payment.TransactionReference = dto.TransactionReference;
        payment.PaymentDate = dto.PaymentDate;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.Status = "Completed";
        payment.ProcessedDate = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        // Update purchase order status to Paid
        payment.GoodsReceipt.PurchaseOrder.Status = "Paid";
        payment.GoodsReceipt.PurchaseOrder.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Payment processed successfully: {payment.TransactionReference}, Amount: {payment.Amount}");

        return Ok(new { message = "Payment processed successfully", transactionReference = payment.TransactionReference });
    }

    // POST: api/payments/{id}/archive
    [HttpPost("{id}/archive")]
    public async Task<ActionResult> ArchivePayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
        {
            return NotFound();
        }

        if (payment.Status != "Completed")
        {
            return BadRequest("Only completed payments can be archived");
        }

        if (payment.IsArchived)
        {
            return BadRequest("Payment is already archived");
        }

        payment.IsArchived = true;
        payment.ArchiveDate = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Payment archived: {payment.TransactionReference}");

        return Ok(new { message = "Payment archived successfully" });
    }

    // GET: api/payments/status/{status}
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<PaymentSummaryDto>>> GetByStatus(string status)
    {
        var payments = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Quotation)
                        .ThenInclude(q => q.Supplier)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentSummaryDto
            {
                Id = p.Id,
                OrderNumber = p.GoodsReceipt.PurchaseOrder.OrderNumber,
                TransactionReference = p.TransactionReference,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                SupplierNameAr = p.GoodsReceipt.PurchaseOrder.Quotation.Supplier.NameAr,
                SupplierNameEn = p.GoodsReceipt.PurchaseOrder.Quotation.Supplier.NameEn
            })
            .ToListAsync();

        return Ok(payments);
    }

    // GET: api/payments/pending-verification
    [HttpGet("pending-verification")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPendingVerification()
    {
        var payments = await _context.Payments
            .Include(p => p.GoodsReceipt)
                .ThenInclude(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Quotation)
                        .ThenInclude(q => q.Supplier)
            .Include(p => p.ProcessedBy)
            .Where(p => p.Status == "Pending" && (!p.DocumentsVerified || !p.QuantityVerified || !p.PriceVerified))
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        var dtos = payments.Select(p => MapToDto(p)).ToList();
        return Ok(dtos);
    }

    private async Task<string> GenerateTransactionReference()
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;
        var lastPayment = await _context.Payments
            .Where(p => p.TransactionReference.StartsWith($"TXN-{year}{month:D2}"))
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastPayment != null)
        {
            var parts = lastPayment.TransactionReference.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1].Substring(6), out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"TXN-{year}{month:D2}{nextNumber:D4}";
    }

    private PaymentDto MapToDto(Payment payment)
    {
        var dto = new PaymentDto
        {
            Id = payment.Id,
            GoodsReceiptId = payment.GoodsReceiptId,
            TransactionReference = payment.TransactionReference,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            DocumentsVerified = payment.DocumentsVerified,
            QuantityVerified = payment.QuantityVerified,
            PriceVerified = payment.PriceVerified,
            ProcessedById = payment.ProcessedById,
            ProcessedDate = payment.ProcessedDate,
            VerificationNotes = payment.VerificationNotes,
            IsArchived = payment.IsArchived,
            ArchiveDate = payment.ArchiveDate,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };

        if (payment.ProcessedBy != null)
        {
            dto.ProcessedByNameAr = payment.ProcessedBy.FullNameAr;
            dto.ProcessedByNameEn = payment.ProcessedBy.FullNameEn;
        }

        if (payment.GoodsReceipt != null)
        {
            dto.GoodsReceipt = new GoodsReceiptDto
            {
                Id = payment.GoodsReceipt.Id,
                PurchaseOrderId = payment.GoodsReceipt.PurchaseOrderId,
                ReceivedDate = payment.GoodsReceipt.ReceivedDate,
                ReceivedQuantity = payment.GoodsReceipt.ReceivedQuantity,
                ExpectedQuantity = payment.GoodsReceipt.ExpectedQuantity,
                QualityStatus = payment.GoodsReceipt.QualityStatus,
                StockUpdated = payment.GoodsReceipt.StockUpdated,
                CreatedAt = payment.GoodsReceipt.CreatedAt,
                UpdatedAt = payment.GoodsReceipt.UpdatedAt
            };

            if (payment.GoodsReceipt.PurchaseOrder != null)
            {
                dto.GoodsReceipt.PurchaseOrder = new PurchaseOrderDto
                {
                    Id = payment.GoodsReceipt.PurchaseOrder.Id,
                    OrderNumber = payment.GoodsReceipt.PurchaseOrder.OrderNumber,
                    Status = payment.GoodsReceipt.PurchaseOrder.Status,
                    TotalAmount = payment.GoodsReceipt.PurchaseOrder.TotalAmount,
                    CreatedAt = payment.GoodsReceipt.PurchaseOrder.CreatedAt,
                    UpdatedAt = payment.GoodsReceipt.PurchaseOrder.UpdatedAt
                };
            }
        }

        return dto;
    }
}
