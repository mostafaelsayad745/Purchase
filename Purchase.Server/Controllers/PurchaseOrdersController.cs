using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.DTOs;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly PurchaseDbContext _context;
    private readonly ILogger<PurchaseOrdersController> _logger;

    public PurchaseOrdersController(PurchaseDbContext context, ILogger<PurchaseOrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/purchaseorders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseOrderSummaryDto>>> GetPurchaseOrders()
    {
        var orders = await _context.PurchaseOrders
            .Include(po => po.Quotation)
                .ThenInclude(q => q.Supplier)
            .Include(po => po.Quotation)
                .ThenInclude(q => q.PurchaseRequest)
                    .ThenInclude(pr => pr.ToolRequest)
                        .ThenInclude(tr => tr.Tool)
            .OrderByDescending(po => po.CreatedAt)
            .Select(po => new PurchaseOrderSummaryDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                SupplierNameAr = po.Quotation.Supplier.NameAr,
                SupplierNameEn = po.Quotation.Supplier.NameEn,
                ToolNameAr = po.Quotation.PurchaseRequest.ToolRequest.Tool.NameAr,
                ToolNameEn = po.Quotation.PurchaseRequest.ToolRequest.Tool.NameEn
            })
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/purchaseorders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetPurchaseOrder(int id)
    {
        var order = await _context.PurchaseOrders
            .Include(po => po.Quotation)
                .ThenInclude(q => q.Supplier)
            .Include(po => po.Quotation)
                .ThenInclude(q => q.PurchaseRequest)
                    .ThenInclude(pr => pr.ToolRequest)
                        .ThenInclude(tr => tr.Tool)
            .Include(po => po.Quotation)
                .ThenInclude(q => q.PurchaseRequest)
                    .ThenInclude(pr => pr.ToolRequest)
                        .ThenInclude(tr => tr.WorkArea)
            .Include(po => po.GoodsReceipt)
            .FirstOrDefaultAsync(po => po.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var dto = MapToDto(order);
        return Ok(dto);
    }

    // POST: api/purchaseorders
    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> CreatePurchaseOrder(CreatePurchaseOrderDto dto)
    {
        // Validate quotation exists and is selected
        var quotation = await _context.Quotations
            .Include(q => q.Supplier)
            .Include(q => q.PurchaseRequest)
                .ThenInclude(pr => pr.ToolRequest)
                    .ThenInclude(tr => tr.Tool)
            .FirstOrDefaultAsync(q => q.Id == dto.QuotationId);

        if (quotation == null)
        {
            return NotFound("Quotation not found");
        }

        if (!quotation.IsSelected)
        {
            return BadRequest("Only selected quotations can be converted to purchase orders");
        }

        // Check if PO already exists for this quotation
        var existingPO = await _context.PurchaseOrders
            .FirstOrDefaultAsync(po => po.QuotationId == dto.QuotationId);

        if (existingPO != null)
        {
            return BadRequest("Purchase order already exists for this quotation");
        }

        // Generate order number
        var orderNumber = await GenerateOrderNumber();

        var purchaseOrder = new PurchaseOrder
        {
            OrderNumber = orderNumber,
            QuotationId = dto.QuotationId,
            OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Status = "Created",
            TotalAmount = quotation.TotalPrice,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        // Load relationships for response
        await _context.Entry(purchaseOrder)
            .Reference(po => po.Quotation)
            .LoadAsync();
        await _context.Entry(purchaseOrder.Quotation)
            .Reference(q => q.Supplier)
            .LoadAsync();

        var result = MapToDto(purchaseOrder);
        return CreatedAtAction(nameof(GetPurchaseOrder), new { id = purchaseOrder.Id }, result);
    }

    // PUT: api/purchaseorders/{id}/documents
    [HttpPut("{id}/documents")]
    public async Task<ActionResult> UpdateDocuments(int id, UpdatePurchaseOrderDto dto)
    {
        var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);

        if (purchaseOrder == null)
        {
            return NotFound();
        }

        // Update documents
        if (!string.IsNullOrEmpty(dto.PurchaseOrderDocument))
            purchaseOrder.PurchaseOrderDocument = dto.PurchaseOrderDocument;
        
        if (!string.IsNullOrEmpty(dto.InvoiceDocument))
            purchaseOrder.InvoiceDocument = dto.InvoiceDocument;
        
        if (!string.IsNullOrEmpty(dto.DeliveryNoteDocument))
            purchaseOrder.DeliveryNoteDocument = dto.DeliveryNoteDocument;
        
        if (!string.IsNullOrEmpty(dto.SupplierAgreementDocument))
            purchaseOrder.SupplierAgreementDocument = dto.SupplierAgreementDocument;
        
        if (!string.IsNullOrEmpty(dto.Notes))
            purchaseOrder.Notes = dto.Notes;

        purchaseOrder.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/purchaseorders/status/{status}
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<PurchaseOrderSummaryDto>>> GetByStatus(string status)
    {
        var orders = await _context.PurchaseOrders
            .Include(po => po.Quotation)
                .ThenInclude(q => q.Supplier)
            .Include(po => po.Quotation)
                .ThenInclude(q => q.PurchaseRequest)
                    .ThenInclude(pr => pr.ToolRequest)
                        .ThenInclude(tr => tr.Tool)
            .Where(po => po.Status == status)
            .OrderByDescending(po => po.CreatedAt)
            .Select(po => new PurchaseOrderSummaryDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                SupplierNameAr = po.Quotation.Supplier.NameAr,
                SupplierNameEn = po.Quotation.Supplier.NameEn,
                ToolNameAr = po.Quotation.PurchaseRequest.ToolRequest.Tool.NameAr,
                ToolNameEn = po.Quotation.PurchaseRequest.ToolRequest.Tool.NameEn
            })
            .ToListAsync();

        return Ok(orders);
    }

    private async Task<string> GenerateOrderNumber()
    {
        var year = DateTime.UtcNow.Year;
        var lastOrder = await _context.PurchaseOrders
            .Where(po => po.OrderNumber.StartsWith($"PO-{year}"))
            .OrderByDescending(po => po.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastOrder != null)
        {
            var parts = lastOrder.OrderNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"PO-{year}-{nextNumber:D4}";
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder order)
    {
        var dto = new PurchaseOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            QuotationId = order.QuotationId,
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            PurchaseOrderDocument = order.PurchaseOrderDocument,
            InvoiceDocument = order.InvoiceDocument,
            DeliveryNoteDocument = order.DeliveryNoteDocument,
            SupplierAgreementDocument = order.SupplierAgreementDocument,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

        if (order.Quotation != null)
        {
            dto.Quotation = new QuotationDto
            {
                Id = order.Quotation.Id,
                PurchaseRequestId = order.Quotation.PurchaseRequestId,
                SupplierId = order.Quotation.SupplierId,
                QuantityOffered = order.Quotation.QuantityOffered,
                UnitPrice = order.Quotation.UnitPrice,
                TotalPrice = order.Quotation.TotalPrice,
                DeliveryTimeDays = order.Quotation.DeliveryTimeDays,
                Notes = order.Quotation.Notes,
                QuotationDate = order.Quotation.QuotationDate,
                Ranking = order.Quotation.Ranking,
                IsSelected = order.Quotation.IsSelected
            };

            if (order.Quotation.Supplier != null)
            {
                dto.Quotation.SupplierNameAr = order.Quotation.Supplier.NameAr;
                dto.Quotation.SupplierNameEn = order.Quotation.Supplier.NameEn;
            }
        }

        if (order.GoodsReceipt != null)
        {
            dto.GoodsReceipt = new GoodsReceiptDto
            {
                Id = order.GoodsReceipt.Id,
                PurchaseOrderId = order.GoodsReceipt.PurchaseOrderId,
                ReceivedDate = order.GoodsReceipt.ReceivedDate,
                ReceivedQuantity = order.GoodsReceipt.ReceivedQuantity,
                ExpectedQuantity = order.GoodsReceipt.ExpectedQuantity,
                QuantityVariancePercentage = order.GoodsReceipt.QuantityVariancePercentage,
                IsQuantityAcceptable = order.GoodsReceipt.IsQuantityAcceptable,
                QualityStatus = order.GoodsReceipt.QualityStatus,
                StockUpdated = order.GoodsReceipt.StockUpdated,
                CreatedAt = order.GoodsReceipt.CreatedAt,
                UpdatedAt = order.GoodsReceipt.UpdatedAt
            };
        }

        return dto;
    }
}
