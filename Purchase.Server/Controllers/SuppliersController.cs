using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly PurchaseDbContext _context;

    public SuppliersController(PurchaseDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
    {
        return await _context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.NameAr)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }
        return supplier;
    }
}
