using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseRequestsController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<PurchaseRequestsController> _logger;

    public PurchaseRequestsController(PurchaseDbContext context, ILogger<PurchaseRequestsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Step 3: Create Purchase Request (First Approval)
    [HttpPost]
    public async Task<ActionResult<PurchaseRequestDto>> CreatePurchaseRequest(CreatePurchaseRequestDto dto)
    {
        try
        {
            // Validate tool request exists and is out of stock
            var toolRequest = await _context.ToolRequests.FindAsync(dto.ToolRequestId);
            if (toolRequest == null)
            {
                return BadRequest(new { message = "طلب الأداة غير موجود", messageEn = "Tool request not found" });
            }

            if (toolRequest.Status != "OutOfStock")
            {
                return BadRequest(new { message = "لا يمكن إنشاء طلب شراء لأداة متوفرة في المخزون", messageEn = "Cannot create purchase request for in-stock item" });
            }

            // Check if purchase request already exists
            var existingRequest = await _context.PurchaseRequests
                .FirstOrDefaultAsync(pr => pr.ToolRequestId == dto.ToolRequestId);
            
            if (existingRequest != null)
            {
                return BadRequest(new { message = "طلب شراء موجود بالفعل لهذا الطلب", messageEn = "Purchase request already exists for this tool request" });
            }

            var purchaseRequest = new PurchaseRequest
            {
                ToolRequestId = dto.ToolRequestId,
                RequestDate = DateTime.UtcNow,
                Status = "PendingApproval",
                EstimatedBudget = dto.EstimatedBudget
            };

            _context.PurchaseRequests.Add(purchaseRequest);
            await _context.SaveChangesAsync();

            var result = await GetPurchaseRequestDto(purchaseRequest.Id);
            return CreatedAtAction(nameof(GetPurchaseRequest), new { id = purchaseRequest.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase request");
            return StatusCode(500, new { message = "خطأ في إنشاء طلب الشراء", messageEn = "Error creating purchase request" });
        }
    }

    [HttpPost("approve")]
    public async Task<ActionResult<PurchaseRequestDto>> ApprovePurchaseRequest(ApprovePurchaseRequestDto dto)
    {
        try
        {
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(dto.PurchaseRequestId);
            if (purchaseRequest == null)
            {
                return NotFound(new { message = "طلب الشراء غير موجود", messageEn = "Purchase request not found" });
            }

            if (purchaseRequest.Status != "PendingApproval")
            {
                return BadRequest(new { message = "لا يمكن الموافقة على طلب شراء تمت معالجته بالفعل", messageEn = "Cannot approve already processed purchase request" });
            }

            purchaseRequest.Status = dto.IsApproved ? "Approved" : "Rejected";
            purchaseRequest.ApprovedById = 1; // TODO: Get from authenticated user
            purchaseRequest.ApprovalDate = DateTime.UtcNow;
            purchaseRequest.ApprovalNotes = dto.Notes;
            purchaseRequest.RejectionReason = dto.IsApproved ? null : dto.RejectionReason;
            purchaseRequest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = await GetPurchaseRequestDto(purchaseRequest.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving purchase request");
            return StatusCode(500, new { message = "خطأ في الموافقة على طلب الشراء", messageEn = "Error approving purchase request" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDto>>> GetPurchaseRequests([FromQuery] string? status = null)
    {
        try
        {
            var query = _context.PurchaseRequests
                .Include(pr => pr.ToolRequest)
                    .ThenInclude(tr => tr.Tool)
                .Include(pr => pr.ApprovedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(pr => pr.Status == status);
            }

            var purchaseRequests = await query
                .OrderByDescending(pr => pr.RequestDate)
                .Select(pr => new PurchaseRequestDto
                {
                    Id = pr.Id,
                    ToolRequestId = pr.ToolRequestId,
                    ToolNameAr = pr.ToolRequest.Tool.NameAr,
                    QuantityNeeded = pr.ToolRequest.QuantityNeeded,
                    RequestDate = pr.RequestDate,
                    Status = pr.Status,
                    ApprovedById = pr.ApprovedById,
                    ApprovedByNameAr = pr.ApprovedBy != null ? pr.ApprovedBy.FullNameAr : null,
                    ApprovalDate = pr.ApprovalDate,
                    ApprovalNotes = pr.ApprovalNotes,
                    RejectionReason = pr.RejectionReason,
                    EstimatedBudget = pr.EstimatedBudget
                })
                .ToListAsync();

            return Ok(purchaseRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase requests");
            return StatusCode(500, new { message = "خطأ في جلب طلبات الشراء", messageEn = "Error getting purchase requests" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseRequestDto>> GetPurchaseRequest(int id)
    {
        try
        {
            var result = await GetPurchaseRequestDto(id);
            if (result == null)
            {
                return NotFound(new { message = "طلب الشراء غير موجود", messageEn = "Purchase request not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase request");
            return StatusCode(500, new { message = "خطأ في جلب طلب الشراء", messageEn = "Error getting purchase request" });
        }
    }

    private async Task<PurchaseRequestDto?> GetPurchaseRequestDto(int id)
    {
        return await _context.PurchaseRequests
            .Include(pr => pr.ToolRequest)
                .ThenInclude(tr => tr.Tool)
            .Include(pr => pr.ApprovedBy)
            .Where(pr => pr.Id == id)
            .Select(pr => new PurchaseRequestDto
            {
                Id = pr.Id,
                ToolRequestId = pr.ToolRequestId,
                ToolNameAr = pr.ToolRequest.Tool.NameAr,
                QuantityNeeded = pr.ToolRequest.QuantityNeeded,
                RequestDate = pr.RequestDate,
                Status = pr.Status,
                ApprovedById = pr.ApprovedById,
                ApprovedByNameAr = pr.ApprovedBy != null ? pr.ApprovedBy.FullNameAr : null,
                ApprovalDate = pr.ApprovalDate,
                ApprovalNotes = pr.ApprovalNotes,
                RejectionReason = pr.RejectionReason,
                EstimatedBudget = pr.EstimatedBudget
            })
            .FirstOrDefaultAsync();
    }
}
