using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.StockTracker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryStockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryStockController(ApplicationDbContext context)
        {
            _context = context;
        }

    [HttpGet("LowStock")]
    public async Task<ActionResult<IEnumerable<LowStockDto>>> GetLowStock()
    {
        var lowStockItems = await _context.InventoryStocks
            .Include(i => i.Product)
            .Include(i => i.Unit)
            .Where(i => i.Quantity < i.ReorderLevel)
            .Select(i => new LowStockDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                UnitName = i.Unit.UnitName,
                Quantity = i.Quantity,
                ReorderLevel = i.ReorderLevel
            })
            .ToListAsync();

        return Ok(lowStockItems);
    }

    [HttpPut("UpdateReorderLevel/{id}")]
    public async Task<IActionResult> UpdateReorderLevel(int id, [FromBody] ProductReorderUpdateDto dto)
    {
        var stock = await _context.InventoryStocks.FindAsync(id);
        if (stock == null) return NotFound();

        stock.ReorderLevel = dto.ReorderLevel ?? stock.ReorderLevel;
        stock.LastUpdated = DateTime.Now;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    }
}