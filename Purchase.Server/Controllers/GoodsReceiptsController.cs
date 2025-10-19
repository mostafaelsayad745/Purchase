using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoodsReceiptsController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<GoodsReceiptsController> _logger;

    public GoodsReceiptsController(PurchaseDbContext context, ILogger<GoodsReceiptsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/goodsreceipts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GoodsReceiptSummaryDto>>> GetGoodsReceipts()
    {
        var receipts = await _context.GoodsReceipts
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
                    .ThenInclude(q => q.PurchaseRequest)
                        .ThenInclude(pr => pr.ToolRequest)
                            .ThenInclude(tr => tr.Tool)
            .OrderByDescending(gr => gr.CreatedAt)
            .Select(gr => new GoodsReceiptSummaryDto
            {
                Id = gr.Id,
                OrderNumber = gr.PurchaseOrder.OrderNumber,
                ReceivedDate = gr.ReceivedDate,
                ReceivedQuantity = gr.ReceivedQuantity,
                ExpectedQuantity = gr.ExpectedQuantity,
                QualityStatus = gr.QualityStatus,
                StockUpdated = gr.StockUpdated,
                ToolNameAr = gr.PurchaseOrder.Quotation.PurchaseRequest.ToolRequest.Tool.NameAr,
                ToolNameEn = gr.PurchaseOrder.Quotation.PurchaseRequest.ToolRequest.Tool.NameEn
            })
            .ToListAsync();

        return Ok(receipts);
    }

    // GET: api/goodsreceipts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GoodsReceiptDto>> GetGoodsReceipt(int id)
    {
        var receipt = await _context.GoodsReceipts
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
                    .ThenInclude(q => q.Supplier)
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
                    .ThenInclude(q => q.PurchaseRequest)
                        .ThenInclude(pr => pr.ToolRequest)
                            .ThenInclude(tr => tr.Tool)
            .Include(gr => gr.ReceivedBy)
            .Include(gr => gr.Payment)
            .FirstOrDefaultAsync(gr => gr.Id == id);

        if (receipt == null)
        {
            return NotFound();
        }

        var dto = MapToDto(receipt);
        return Ok(dto);
    }

    // POST: api/goodsreceipts
    [HttpPost]
    public async Task<ActionResult<GoodsReceiptDto>> CreateGoodsReceipt(CreateGoodsReceiptDto dto)
    {
        // Validate purchase order exists
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Quotation)
                .ThenInclude(q => q.PurchaseRequest)
                    .ThenInclude(pr => pr.ToolRequest)
                        .ThenInclude(tr => tr.Tool)
            .FirstOrDefaultAsync(po => po.Id == dto.PurchaseOrderId);

        if (purchaseOrder == null)
        {
            return NotFound("Purchase order not found");
        }

        if (purchaseOrder.Status != "Created")
        {
            return BadRequest("Goods can only be received for purchase orders with 'Created' status");
        }

        // Check if goods receipt already exists
        var existingReceipt = await _context.GoodsReceipts
            .FirstOrDefaultAsync(gr => gr.PurchaseOrderId == dto.PurchaseOrderId);

        if (existingReceipt != null)
        {
            return BadRequest("Goods receipt already exists for this purchase order");
        }

        // Calculate quantity variance
        var expectedQuantity = purchaseOrder.Quotation.QuantityOffered;
        var variancePercentage = ((decimal)(dto.ReceivedQuantity - expectedQuantity) / expectedQuantity) * 100;
        var isAcceptable = Math.Abs(variancePercentage) <= 5; // ±5% tolerance

        var goodsReceipt = new GoodsReceipt
        {
            PurchaseOrderId = dto.PurchaseOrderId,
            ReceivedDate = DateTime.UtcNow,
            ReceivedQuantity = dto.ReceivedQuantity,
            ExpectedQuantity = expectedQuantity,
            QuantityVariancePercentage = variancePercentage,
            IsQuantityAcceptable = isAcceptable,
            QualityStatus = dto.QualityStatus,
            QualityNotes = dto.QualityNotes,
            ConditionNotes = dto.ConditionNotes,
            ProofOfReceiptDocument = dto.ProofOfReceiptDocument,
            ReceivedById = dto.ReceivedById,
            StockUpdated = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.GoodsReceipts.Add(goodsReceipt);

        // Update purchase order status
        purchaseOrder.Status = "Received";
        purchaseOrder.UpdatedAt = DateTime.UtcNow;

        // Auto-update stock if quality is Accepted and quantity is acceptable
        if (dto.QualityStatus == "Accepted" && isAcceptable)
        {
            var tool = purchaseOrder.Quotation.PurchaseRequest.ToolRequest.Tool;
            tool.CurrentQuantity += dto.ReceivedQuantity;
            tool.UpdatedAt = DateTime.UtcNow;

            goodsReceipt.StockUpdated = true;
            goodsReceipt.StockUpdateDate = DateTime.UtcNow;

            // Update purchase order status
            purchaseOrder.Status = "Inspected";

            _logger.LogInformation($"Stock updated for tool {tool.ToolId}: Added {dto.ReceivedQuantity}, New quantity: {tool.CurrentQuantity}");
        }

        await _context.SaveChangesAsync();

        // Load relationships for response
        await _context.Entry(goodsReceipt)
            .Reference(gr => gr.PurchaseOrder)
            .LoadAsync();
        await _context.Entry(goodsReceipt.PurchaseOrder)
            .Reference(po => po.Quotation)
            .LoadAsync();

        var result = MapToDto(goodsReceipt);
        return CreatedAtAction(nameof(GetGoodsReceipt), new { id = goodsReceipt.Id }, result);
    }

    // PUT: api/goodsreceipts/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGoodsReceipt(int id, UpdateGoodsReceiptDto dto)
    {
        var goodsReceipt = await _context.GoodsReceipts
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
                    .ThenInclude(q => q.PurchaseRequest)
                        .ThenInclude(pr => pr.ToolRequest)
                            .ThenInclude(tr => tr.Tool)
            .FirstOrDefaultAsync(gr => gr.Id == id);

        if (goodsReceipt == null)
        {
            return NotFound();
        }

        var oldQualityStatus = goodsReceipt.QualityStatus;

        // Update fields
        goodsReceipt.QualityStatus = dto.QualityStatus;
        goodsReceipt.QualityNotes = dto.QualityNotes;
        goodsReceipt.ConditionNotes = dto.ConditionNotes;
        if (!string.IsNullOrEmpty(dto.ProofOfReceiptDocument))
            goodsReceipt.ProofOfReceiptDocument = dto.ProofOfReceiptDocument;
        goodsReceipt.UpdatedAt = DateTime.UtcNow;

        // Update stock if status changed to Accepted and not already updated
        if (oldQualityStatus != "Accepted" && dto.QualityStatus == "Accepted" && 
            !goodsReceipt.StockUpdated && goodsReceipt.IsQuantityAcceptable)
        {
            var tool = goodsReceipt.PurchaseOrder.Quotation.PurchaseRequest.ToolRequest.Tool;
            tool.CurrentQuantity += goodsReceipt.ReceivedQuantity;
            tool.UpdatedAt = DateTime.UtcNow;

            goodsReceipt.StockUpdated = true;
            goodsReceipt.StockUpdateDate = DateTime.UtcNow;

            // Update purchase order status
            goodsReceipt.PurchaseOrder.Status = "Inspected";
            goodsReceipt.PurchaseOrder.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation($"Stock updated for tool {tool.ToolId}: Added {goodsReceipt.ReceivedQuantity}, New quantity: {tool.CurrentQuantity}");
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/goodsreceipts/{id}/update-stock
    [HttpPost("{id}/update-stock")]
    public async Task<ActionResult> UpdateStock(int id)
    {
        var goodsReceipt = await _context.GoodsReceipts
            .Include(gr => gr.PurchaseOrder)
                .ThenInclude(po => po.Quotation)
                    .ThenInclude(q => q.PurchaseRequest)
                        .ThenInclude(pr => pr.ToolRequest)
                            .ThenInclude(tr => tr.Tool)
            .FirstOrDefaultAsync(gr => gr.Id == id);

        if (goodsReceipt == null)
        {
            return NotFound();
        }

        if (goodsReceipt.StockUpdated)
        {
            return BadRequest("Stock has already been updated for this goods receipt");
        }

        if (goodsReceipt.QualityStatus != "Accepted")
        {
            return BadRequest("Stock can only be updated for accepted goods");
        }

        if (!goodsReceipt.IsQuantityAcceptable)
        {
            return BadRequest("Stock can only be updated for goods with acceptable quantity variance (±5%)");
        }

        var tool = goodsReceipt.PurchaseOrder.Quotation.PurchaseRequest.ToolRequest.Tool;
        tool.CurrentQuantity += goodsReceipt.ReceivedQuantity;
        tool.UpdatedAt = DateTime.UtcNow;

        goodsReceipt.StockUpdated = true;
        goodsReceipt.StockUpdateDate = DateTime.UtcNow;
        goodsReceipt.UpdatedAt = DateTime.UtcNow;

        // Update purchase order status
        goodsReceipt.PurchaseOrder.Status = "Inspected";
        goodsReceipt.PurchaseOrder.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Stock manually updated for tool {tool.ToolId}: Added {goodsReceipt.ReceivedQuantity}, New quantity: {tool.CurrentQuantity}");

        return Ok(new { message = "Stock updated successfully", newQuantity = tool.CurrentQuantity });
    }

    private GoodsReceiptDto MapToDto(GoodsReceipt receipt)
    {
        var dto = new GoodsReceiptDto
        {
            Id = receipt.Id,
            PurchaseOrderId = receipt.PurchaseOrderId,
            ReceivedDate = receipt.ReceivedDate,
            ReceivedQuantity = receipt.ReceivedQuantity,
            ExpectedQuantity = receipt.ExpectedQuantity,
            QuantityVariancePercentage = receipt.QuantityVariancePercentage,
            IsQuantityAcceptable = receipt.IsQuantityAcceptable,
            QualityStatus = receipt.QualityStatus,
            QualityNotes = receipt.QualityNotes,
            ConditionNotes = receipt.ConditionNotes,
            ProofOfReceiptDocument = receipt.ProofOfReceiptDocument,
            ReceivedById = receipt.ReceivedById,
            StockUpdated = receipt.StockUpdated,
            StockUpdateDate = receipt.StockUpdateDate,
            CreatedAt = receipt.CreatedAt,
            UpdatedAt = receipt.UpdatedAt
        };

        if (receipt.ReceivedBy != null)
        {
            dto.ReceivedByNameAr = receipt.ReceivedBy.FullNameAr;
            dto.ReceivedByNameEn = receipt.ReceivedBy.FullNameEn;
        }

        if (receipt.PurchaseOrder != null)
        {
            dto.PurchaseOrder = new PurchaseOrderDto
            {
                Id = receipt.PurchaseOrder.Id,
                OrderNumber = receipt.PurchaseOrder.OrderNumber,
                QuotationId = receipt.PurchaseOrder.QuotationId,
                OrderDate = receipt.PurchaseOrder.OrderDate,
                ExpectedDeliveryDate = receipt.PurchaseOrder.ExpectedDeliveryDate,
                Status = receipt.PurchaseOrder.Status,
                TotalAmount = receipt.PurchaseOrder.TotalAmount,
                CreatedAt = receipt.PurchaseOrder.CreatedAt,
                UpdatedAt = receipt.PurchaseOrder.UpdatedAt
            };
        }

        if (receipt.Payment != null)
        {
            dto.Payment = new PaymentDto
            {
                Id = receipt.Payment.Id,
                GoodsReceiptId = receipt.Payment.GoodsReceiptId,
                TransactionReference = receipt.Payment.TransactionReference,
                Amount = receipt.Payment.Amount,
                PaymentDate = receipt.Payment.PaymentDate,
                PaymentMethod = receipt.Payment.PaymentMethod,
                Status = receipt.Payment.Status,
                CreatedAt = receipt.Payment.CreatedAt,
                UpdatedAt = receipt.Payment.UpdatedAt
            };
        }

        return dto;
    }
}
