using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.StockTracker;
using katsuCMS_backend.Models.DTO.InventoryStock;
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
        public async Task<ActionResult<IEnumerable<InventoryStockDto>>> GetLowStock()
        {
            var lowStockItems = await _context.InventoryStocks
                .Include(i => i.Product)
                .Include(i => i.Product.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(i => i.Unit)
                .Where(i => i.Quantity < i.ReorderLevel)
                .Select(i => new InventoryStockDto
                {
                    ProductId = i.ProductId,
                    ProductCode = i.Product.ProductCode,
                    ProductName = i.Product.ProductName,
                    UnitName = i.Unit.UnitName,
                    Quantity = i.Quantity,
                    ReorderLevel = i.ReorderLevel,
                    LastUpdated = i.LastUpdated,
                    IsLowstock = i.Quantity < i.ReorderLevel,
                    SupplierNames = i.Product.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
                })
                .ToListAsync();

            return Ok(lowStockItems);
        }
    
        [HttpPost("AddStock")]
        public async Task<IActionResult> AddStock([FromBody] InventoryStockCreateDto dto)
        {
            var stock = new InventoryStock
            {
                ProductId = dto.ProductId,
                UnitId = dto.UnitId,
                Quantity = dto.Quantity,
                ReorderLevel = dto.ReorderLevel,
                PreferredStockLevel = dto.PreferredStockLevel,
            
                LastUpdated = DateTime.Now
            };
            _context.InventoryStocks.Add(stock);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLowStock), new { id = stock.Id }, stock);
        }

        [HttpPut("UpdateReorderLevel/{id}")]
        public async Task<IActionResult> UpdateReorderLevel(int id, [FromBody] InventoryStockUpdateDto dto)
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