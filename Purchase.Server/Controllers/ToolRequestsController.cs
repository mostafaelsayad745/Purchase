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

    [HttpPost]
    public async Task<ActionResult<ToolRequestDto>> CreateToolRequest(CreateToolRequestDto dto)
    {
        try
        {
            int toolId;
            int workAreaId;
            int requesterId = 1; // Default requester

            // Handle tool - accept either ToolId or ToolName
            if (dto.ToolId.HasValue)
            {
                toolId = dto.ToolId.Value;
                var tool = await _context.StockItems.FindAsync(toolId);
                if (tool == null)
                {
                    return BadRequest(new { message = "الأداة المحددة غير موجودة", messageEn = "Tool not found" });
                }
            }
            else if (!string.IsNullOrEmpty(dto.ToolName))
            {
                // Try to find by name, or create a new one if needed
                var tool = await _context.StockItems.FirstOrDefaultAsync(t => t.NameAr == dto.ToolName);
                if (tool == null)
                {
                    // Create a new stock item
                    var newTool = new StockItem
                    {
                        NameAr = dto.ToolName,
                        NameEn = dto.ToolName,
                        CurrentQuantity = 0,
                        MinimumQuantity = 1,
                        Unit = "قطعة"
                    };
                    _context.StockItems.Add(newTool);
                    await _context.SaveChangesAsync();
                    toolId = newTool.Id;
                }
                else
                {
                    toolId = tool.Id;
                }
            }
            else
            {
                return BadRequest(new { message = "يجب تحديد الأداة إما بالمعرف أو الاسم", messageEn = "Tool must be specified by ID or name" });
            }

            // Handle work area - accept either WorkAreaId or WorkAreaName
            if (dto.WorkAreaId.HasValue)
            {
                workAreaId = dto.WorkAreaId.Value;
                var workArea = await _context.WorkAreas.FindAsync(workAreaId);
                if (workArea == null)
                {
                    return BadRequest(new { message = "منطقة العمل المحددة غير موجودة", messageEn = "Work area not found" });
                }
            }
            else if (!string.IsNullOrEmpty(dto.WorkAreaName))
            {
                // Try to find by name, or create a new one if needed
                var workArea = await _context.WorkAreas.FirstOrDefaultAsync(w => w.NameAr == dto.WorkAreaName);
                if (workArea == null)
                {
                    // Create a new work area
                    var newWorkArea = new WorkArea
                    {
                        NameAr = dto.WorkAreaName,
                        NameEn = dto.WorkAreaName,
                        Location = "غير محدد"
                    };
                    _context.WorkAreas.Add(newWorkArea);
                    await _context.SaveChangesAsync();
                    workAreaId = newWorkArea.Id;
                }
                else
                {
                    workAreaId = workArea.Id;
                }
            }
            else
            {
                return BadRequest(new { message = "يجب تحديد منطقة العمل إما بالمعرف أو الاسم", messageEn = "Work area must be specified by ID or name" });
            }

            // Handle requester - accept either default or RequesterName
            if (!string.IsNullOrEmpty(dto.RequesterName))
            {
                var requester = await _context.Users.FirstOrDefaultAsync(u => u.FullNameAr == dto.RequesterName);
                if (requester == null)
                {
                    // Create a new user
                    var newRequester = new User
                    {
                        Username = dto.RequesterName.Replace(" ", "").ToLower(),
                        FullNameAr = dto.RequesterName,
                        FullNameEn = dto.RequesterName,
                        Role = "Employee"
                    };
                    _context.Users.Add(newRequester);
                    await _context.SaveChangesAsync();
                    requesterId = newRequester.Id;
                }
                else
                {
                    requesterId = requester.Id;
                }
            }

            var toolRequest = new ToolRequest
            {
                ToolId = toolId,
                QuantityNeeded = dto.QuantityNeeded,
                WorkAreaId = workAreaId,
                RequesterId = requesterId,
                RequestDate = DateTime.UtcNow,
                ReasonAr = dto.ReasonAr,
                ReasonEn = dto.ReasonEn,
                Status = !string.IsNullOrEmpty(dto.Status) ? dto.Status : "Pending",
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
