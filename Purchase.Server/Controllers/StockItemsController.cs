using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockItemsController : ControllerBase
{
    private readonly PurchaseDbContext _context;

    public StockItemsController(PurchaseDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetStockItems()
    {
        return await _context.StockItems
            .OrderBy(s => s.NameAr)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockItem>> GetStockItem(int id)
    {
        var stockItem = await _context.StockItems.FindAsync(id);
        if (stockItem == null)
        {
            return NotFound();
        }
        return stockItem;
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetLowStockItems()
    {
        return await _context.StockItems
            .Where(s => s.CurrentQuantity <= s.MinimumThreshold)
            .OrderBy(s => s.CurrentQuantity)
            .ToListAsync();
    }
}
