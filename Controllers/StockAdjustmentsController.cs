using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.InventoryStock;
using katsuCMS_backend.Models.DTO.StockAdjustments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockAdjustmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockAdjustmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

[HttpGet]
public async Task<ActionResult<IEnumerable<StockAdjustmentsDto>>> GetAdjustments()
{
    var adjustments = await _context.StockAdjustments
        .Include(a => a.InventoryStock)
            .ThenInclude(p => p!.Product)
        .Include(a => a.InventoryStock!.Unit)
        .Select(a => new StockAdjustmentsDto
        {
            Id = a.Id,
            ProductName = a.InventoryStock!.Product!.ProductName,
            UnitName = a.InventoryStock!.Unit!.UnitName,
            PreviousQuantity = a.PreviousQuantity,
            AdjustedQuantity = a.AdjustedQuantity,
            AdjustmentType = a.AdjustmentType,
            Reason = a.Reason,
            AdjustmentDate = a.AdjustmentDate
        })
        .ToListAsync();

    return Ok(adjustments);
}

        [HttpPost]
        public async Task<IActionResult> CreateAdjustments([FromBody] StockAdjustmentsCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventoryStock = await _context.InventoryStocks.FindAsync(dto.InventoryStockId);
            if (inventoryStock == null)
                return NotFound("Inventory stock not found.");

            var product = await _context.Products.FindAsync(inventoryStock.ProductId);
            if (product == null)
                return NotFound("Product not found.");

            var adjustment = new StockAdjustments
            {
                InventoryStockId = dto.InventoryStockId,
                InventoryStock = inventoryStock,
                ProductId = product.Id,
                Product = product,
                AdjustmentType = dto.AdjustmentType,
                AdjustedQuantity = dto.AdjustedQuantity,
                Reason = dto.Reason,
                PreviousQuantity = inventoryStock.Quantity,
                AdjustmentDate = DateTime.Now
            };

            _context.StockAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}