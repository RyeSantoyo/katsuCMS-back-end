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

        [HttpGet("StockAdjustment")]
        public async Task<ActionResult<IEnumerable<StockAdjustmentsDto>>> GetAdjustments()
        {
            var adjustments = await _context.StockAdjustments
                                    .Include(a => a.InventoryStock)
                                        .ThenInclude(p => p.Product)
                                    .Include(a => a.InventoryStock.Unit)
                                    .Select(a => new StockAdjustmentsDto
                                    {
                                        Id = a.Id,
                                        ProductName = a.InventoryStock.Product.ProductName,
                                        UnitName = a.InventoryStock.Unit.UnitName,
                                        PreviousQuantity = a.PreviousQuantity,
                                        AdjustedQuantity = a.AdjustedQuantity,
                                        AdjustmentType = a.AdjustmentType,
                                        Reason = a.Reason,
                                        AdjustmentDate = a.AdjustmentDate
                                    }).ToListAsync();

            return Ok(adjustments);
        }

        [HttpPost]

        public async Task<IActionResult> CreateAdjustments([FromBody] StockAdjustments dto)
        {

            var stock = await _context.InventoryStocks
                .Include(s => s.Product)
                .Include(s => s.Unit)
                .FirstOrDefaultAsync(s => s.Id == dto.InventoryStockId);

            if (stock == null) return NotFound("Not Found");

            var adjustments = new StockAdjustments
            {
                InventoryStockId = dto.InventoryStockId,
                PreviousQuantity = dto.PreviousQuantity,
                AdjustedQuantity = dto.AdjustedQuantity,
                Reason = dto.Reason,
                AdjustmentDate = dto.AdjustmentDate,
                AdjustmentType = dto.AdjustmentType,
            };

            stock.Quantity = dto.AdjustedQuantity;
            stock.LastUpdated = dto.AdjustmentDate;

            _context.StockAdjustments.Add(adjustments);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAdjustments), new { id = adjustments.Id }, adjustments);


            }

    }
}