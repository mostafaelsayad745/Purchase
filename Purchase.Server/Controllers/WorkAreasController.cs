using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase.Server.Data;
using Purchase.Server.Models;

namespace Purchase.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkAreasController : ControllerBase
{
    private readonly PurchaseDbContext _context;

    public WorkAreasController(PurchaseDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkArea>>> GetWorkAreas()
    {
        return await _context.WorkAreas
            .Where(w => w.IsActive)
            .OrderBy(w => w.NameAr)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkArea>> GetWorkArea(int id)
    {
        var workArea = await _context.WorkAreas.FindAsync(id);
        if (workArea == null)
        {
            return NotFound();
        }
        return workArea;
    }
}
