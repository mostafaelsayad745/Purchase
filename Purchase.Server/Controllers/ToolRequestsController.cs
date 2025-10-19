using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToolRequestsController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<ToolRequestsController> _logger;

    public ToolRequestsController(PurchaseDbContext context, ILogger<ToolRequestsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Step 1: Create Tool Request
    [HttpPost]
    public async Task<ActionResult<ToolRequestDto>> CreateToolRequest(CreateToolRequestDto dto)
    {
        try
        {
            // Validate tool exists
            var tool = await _context.StockItems.FindAsync(dto.ToolId);
            if (tool == null)
            {
                return BadRequest(new { message = "الأداة المحددة غير موجودة", messageEn = "Tool not found" });
            }

            // Validate work area exists
            var workArea = await _context.WorkAreas.FindAsync(dto.WorkAreaId);
            if (workArea == null)
            {
                return BadRequest(new { message = "منطقة العمل المحددة غير موجودة", messageEn = "Work area not found" });
            }

            // For demo purposes, using user ID 1 as requester
            var toolRequest = new ToolRequest
            {
                ToolId = dto.ToolId,
                QuantityNeeded = dto.QuantityNeeded,
                WorkAreaId = dto.WorkAreaId,
                RequesterId = 1, // TODO: Get from authenticated user
                RequestDate = DateTime.UtcNow,
                ReasonAr = dto.ReasonAr,
                ReasonEn = dto.ReasonEn,
                Status = "Pending",
                IsInStock = false
            };

            _context.ToolRequests.Add(toolRequest);
            await _context.SaveChangesAsync();

            var result = await GetToolRequestDto(toolRequest.Id);
            return CreatedAtAction(nameof(GetToolRequest), new { id = toolRequest.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tool request");
            return StatusCode(500, new { message = "خطأ في إنشاء طلب الأداة", messageEn = "Error creating tool request" });
        }
    }

    // Step 2: Stock Verification
    [HttpPost("verify-stock")]
    public async Task<ActionResult<ToolRequestDto>> VerifyStock(StockVerificationDto dto)
    {
        try
        {
            var toolRequest = await _context.ToolRequests
                .Include(tr => tr.Tool)
                .FirstOrDefaultAsync(tr => tr.Id == dto.ToolRequestId);

            if (toolRequest == null)
            {
                return NotFound(new { message = "طلب الأداة غير موجود", messageEn = "Tool request not found" });
            }

            toolRequest.IsInStock = dto.IsInStock;
            toolRequest.StockVerificationNotes = dto.StockVerificationNotes;
            toolRequest.UpdatedAt = DateTime.UtcNow;

            if (dto.IsInStock)
            {
                // If in stock, fulfill directly
                toolRequest.Status = "InStock";
                toolRequest.QuantityFulfilled = dto.QuantityFulfilled ?? toolRequest.QuantityNeeded;
                toolRequest.FulfilledDate = DateTime.UtcNow;

                // Update stock quantity
                var tool = toolRequest.Tool;
                tool.CurrentQuantity -= toolRequest.QuantityFulfilled.Value;
                tool.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // If out of stock, proceed to purchase request
                toolRequest.Status = "OutOfStock";
            }

            await _context.SaveChangesAsync();

            var result = await GetToolRequestDto(toolRequest.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying stock");
            return StatusCode(500, new { message = "خطأ في التحقق من المخزون", messageEn = "Error verifying stock" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToolRequestDto>>> GetToolRequests([FromQuery] string? status = null)
    {
        try
        {
            var query = _context.ToolRequests
                .Include(tr => tr.Tool)
                .Include(tr => tr.WorkArea)
                .Include(tr => tr.Requester)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(tr => tr.Status == status);
            }

            var toolRequests = await query
                .OrderByDescending(tr => tr.RequestDate)
                .Select(tr => new ToolRequestDto
                {
                    Id = tr.Id,
                    ToolId = tr.ToolId,
                    ToolNameAr = tr.Tool.NameAr,
                    ToolNameEn = tr.Tool.NameEn,
                    QuantityNeeded = tr.QuantityNeeded,
                    WorkAreaId = tr.WorkAreaId,
                    WorkAreaNameAr = tr.WorkArea.NameAr,
                    WorkAreaNameEn = tr.WorkArea.NameEn,
                    RequesterId = tr.RequesterId,
                    RequesterNameAr = tr.Requester.FullNameAr,
                    RequestDate = tr.RequestDate,
                    ReasonAr = tr.ReasonAr,
                    ReasonEn = tr.ReasonEn,
                    Status = tr.Status,
                    IsInStock = tr.IsInStock,
                    QuantityFulfilled = tr.QuantityFulfilled,
                    FulfilledDate = tr.FulfilledDate,
                    StockVerificationNotes = tr.StockVerificationNotes
                })
                .ToListAsync();

            return Ok(toolRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tool requests");
            return StatusCode(500, new { message = "خطأ في جلب طلبات الأدوات", messageEn = "Error getting tool requests" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ToolRequestDto>> GetToolRequest(int id)
    {
        try
        {
            var result = await GetToolRequestDto(id);
            if (result == null)
            {
                return NotFound(new { message = "طلب الأداة غير موجود", messageEn = "Tool request not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tool request");
            return StatusCode(500, new { message = "خطأ في جلب طلب الأداة", messageEn = "Error getting tool request" });
        }
    }

    private async Task<ToolRequestDto?> GetToolRequestDto(int id)
    {
        return await _context.ToolRequests
            .Include(tr => tr.Tool)
            .Include(tr => tr.WorkArea)
            .Include(tr => tr.Requester)
            .Where(tr => tr.Id == id)
            .Select(tr => new ToolRequestDto
            {
                Id = tr.Id,
                ToolId = tr.ToolId,
                ToolNameAr = tr.Tool.NameAr,
                ToolNameEn = tr.Tool.NameEn,
                QuantityNeeded = tr.QuantityNeeded,
                WorkAreaId = tr.WorkAreaId,
                WorkAreaNameAr = tr.WorkArea.NameAr,
                WorkAreaNameEn = tr.WorkArea.NameEn,
                RequesterId = tr.RequesterId,
                RequesterNameAr = tr.Requester.FullNameAr,
                RequestDate = tr.RequestDate,
                ReasonAr = tr.ReasonAr,
                ReasonEn = tr.ReasonEn,
                Status = tr.Status,
                IsInStock = tr.IsInStock,
                QuantityFulfilled = tr.QuantityFulfilled,
                FulfilledDate = tr.FulfilledDate,
                StockVerificationNotes = tr.StockVerificationNotes
            })
            .FirstOrDefaultAsync();
    }
}
