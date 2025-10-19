using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationsController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<QuotationsController> _logger;

    public QuotationsController(PurchaseDbContext context, ILogger<QuotationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Step 4: Create Quotation
    [HttpPost]
    public async Task<ActionResult<QuotationDto>> CreateQuotation(CreateQuotationDto dto)
    {
        try
        {
            // Validate purchase request exists and is approved
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(dto.PurchaseRequestId);
            if (purchaseRequest == null)
            {
                return BadRequest(new { message = "طلب الشراء غير موجود", messageEn = "Purchase request not found" });
            }

            if (purchaseRequest.Status != "Approved")
            {
                return BadRequest(new { message = "لا يمكن إضافة عرض سعر لطلب شراء غير موافق عليه", messageEn = "Cannot add quotation for unapproved purchase request" });
            }

            // Validate supplier exists
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
            {
                return BadRequest(new { message = "المورد غير موجود", messageEn = "Supplier not found" });
            }

            var totalPrice = dto.QuantityOffered * dto.UnitPrice;

            var quotation = new Quotation
            {
                PurchaseRequestId = dto.PurchaseRequestId,
                SupplierId = dto.SupplierId,
                QuantityOffered = dto.QuantityOffered,
                UnitPrice = dto.UnitPrice,
                TotalPrice = totalPrice,
                DeliveryTimeDays = dto.DeliveryTimeDays,
                Notes = dto.Notes,
                QuotationDate = DateTime.UtcNow
            };

            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();

            // Recalculate rankings for this purchase request
            await RecalculateRankings(dto.PurchaseRequestId);

            var result = await GetQuotationDto(quotation.Id);
            return CreatedAtAction(nameof(GetQuotation), new { id = quotation.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quotation");
            return StatusCode(500, new { message = "خطأ في إنشاء عرض السعر", messageEn = "Error creating quotation" });
        }
    }

    // Step 4: Get Top 3 Quotations
    [HttpGet("purchase-request/{purchaseRequestId}/top3")]
    public async Task<ActionResult<IEnumerable<QuotationDto>>> GetTop3Quotations(int purchaseRequestId)
    {
        try
        {
            var quotations = await _context.Quotations
                .Include(q => q.Supplier)
                .Where(q => q.PurchaseRequestId == purchaseRequestId)
                .OrderBy(q => q.TotalPrice) // Price-based ranking
                .ThenBy(q => q.DeliveryTimeDays)
                .Take(3)
                .Select(q => new QuotationDto
                {
                    Id = q.Id,
                    PurchaseRequestId = q.PurchaseRequestId,
                    SupplierId = q.SupplierId,
                    SupplierNameAr = q.Supplier.NameAr,
                    SupplierNameEn = q.Supplier.NameEn,
                    QuantityOffered = q.QuantityOffered,
                    UnitPrice = q.UnitPrice,
                    TotalPrice = q.TotalPrice,
                    DeliveryTimeDays = q.DeliveryTimeDays,
                    Notes = q.Notes,
                    QuotationDate = q.QuotationDate,
                    Ranking = q.Ranking,
                    CustomScore = q.CustomScore,
                    IsSelected = q.IsSelected,
                    SelectionReason = q.SelectionReason,
                    RejectionReason = q.RejectionReason
                })
                .ToListAsync();

            return Ok(quotations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top 3 quotations");
            return StatusCode(500, new { message = "خطأ في جلب أفضل 3 عروض أسعار", messageEn = "Error getting top 3 quotations" });
        }
    }

    // Step 5: Select Best Offer (Second Approval)
    [HttpPost("select")]
    public async Task<ActionResult<QuotationDto>> SelectQuotation(SelectQuotationDto dto)
    {
        try
        {
            var quotation = await _context.Quotations
                .Include(q => q.PurchaseRequest)
                .FirstOrDefaultAsync(q => q.Id == dto.QuotationId);

            if (quotation == null)
            {
                return NotFound(new { message = "عرض السعر غير موجود", messageEn = "Quotation not found" });
            }

            // Deselect any previously selected quotation for this purchase request
            var previouslySelected = await _context.Quotations
                .Where(q => q.PurchaseRequestId == quotation.PurchaseRequestId && q.IsSelected)
                .ToListAsync();

            foreach (var prev in previouslySelected)
            {
                prev.IsSelected = false;
                prev.UpdatedAt = DateTime.UtcNow;
            }

            // Select the new quotation
            quotation.IsSelected = true;
            quotation.SelectedById = 1; // TODO: Get from authenticated user
            quotation.SelectionDate = DateTime.UtcNow;
            quotation.SelectionReason = dto.SelectionReason;
            quotation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = await GetQuotationDto(quotation.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting quotation");
            return StatusCode(500, new { message = "خطأ في اختيار عرض السعر", messageEn = "Error selecting quotation" });
        }
    }

    [HttpPost("reject")]
    public async Task<ActionResult<QuotationDto>> RejectQuotation(RejectQuotationDto dto)
    {
        try
        {
            var quotation = await _context.Quotations.FindAsync(dto.QuotationId);
            if (quotation == null)
            {
                return NotFound(new { message = "عرض السعر غير موجود", messageEn = "Quotation not found" });
            }

            quotation.RejectionReason = dto.RejectionReason;
            quotation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = await GetQuotationDto(quotation.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting quotation");
            return StatusCode(500, new { message = "خطأ في رفض عرض السعر", messageEn = "Error rejecting quotation" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuotationDto>>> GetQuotations([FromQuery] int? purchaseRequestId = null)
    {
        try
        {
            var query = _context.Quotations
                .Include(q => q.Supplier)
                .AsQueryable();

            if (purchaseRequestId.HasValue)
            {
                query = query.Where(q => q.PurchaseRequestId == purchaseRequestId.Value);
            }

            var quotations = await query
                .OrderBy(q => q.PurchaseRequestId)
                .ThenBy(q => q.Ranking ?? 999)
                .ThenBy(q => q.TotalPrice)
                .Select(q => new QuotationDto
                {
                    Id = q.Id,
                    PurchaseRequestId = q.PurchaseRequestId,
                    SupplierId = q.SupplierId,
                    SupplierNameAr = q.Supplier.NameAr,
                    SupplierNameEn = q.Supplier.NameEn,
                    QuantityOffered = q.QuantityOffered,
                    UnitPrice = q.UnitPrice,
                    TotalPrice = q.TotalPrice,
                    DeliveryTimeDays = q.DeliveryTimeDays,
                    Notes = q.Notes,
                    QuotationDate = q.QuotationDate,
                    Ranking = q.Ranking,
                    CustomScore = q.CustomScore,
                    IsSelected = q.IsSelected,
                    SelectionReason = q.SelectionReason,
                    RejectionReason = q.RejectionReason
                })
                .ToListAsync();

            return Ok(quotations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quotations");
            return StatusCode(500, new { message = "خطأ في جلب عروض الأسعار", messageEn = "Error getting quotations" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuotationDto>> GetQuotation(int id)
    {
        try
        {
            var result = await GetQuotationDto(id);
            if (result == null)
            {
                return NotFound(new { message = "عرض السعر غير موجود", messageEn = "Quotation not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quotation");
            return StatusCode(500, new { message = "خطأ في جلب عرض السعر", messageEn = "Error getting quotation" });
        }
    }

    private async Task<QuotationDto?> GetQuotationDto(int id)
    {
        return await _context.Quotations
            .Include(q => q.Supplier)
            .Where(q => q.Id == id)
            .Select(q => new QuotationDto
            {
                Id = q.Id,
                PurchaseRequestId = q.PurchaseRequestId,
                SupplierId = q.SupplierId,
                SupplierNameAr = q.Supplier.NameAr,
                SupplierNameEn = q.Supplier.NameEn,
                QuantityOffered = q.QuantityOffered,
                UnitPrice = q.UnitPrice,
                TotalPrice = q.TotalPrice,
                DeliveryTimeDays = q.DeliveryTimeDays,
                Notes = q.Notes,
                QuotationDate = q.QuotationDate,
                Ranking = q.Ranking,
                CustomScore = q.CustomScore,
                IsSelected = q.IsSelected,
                SelectionReason = q.SelectionReason,
                RejectionReason = q.RejectionReason
            })
            .FirstOrDefaultAsync();
    }

    private async Task RecalculateRankings(int purchaseRequestId)
    {
        var quotations = await _context.Quotations
            .Where(q => q.PurchaseRequestId == purchaseRequestId)
            .OrderBy(q => q.TotalPrice)
            .ThenBy(q => q.DeliveryTimeDays)
            .ToListAsync();

        int rank = 1;
        foreach (var quotation in quotations.Take(3))
        {
            quotation.Ranking = rank++;
            quotation.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
