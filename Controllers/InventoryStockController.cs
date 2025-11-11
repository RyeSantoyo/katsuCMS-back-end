using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.StockTracker;
using katsuCMS_backend.Models.DTO.InventoryStock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using katsuCMS_backend.Models.DTO.Product;
using System.Diagnostics;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryStockDto>>> GetCurrentStock()
        {
            var watch = new Stopwatch();
            watch.Start();
            var stocks = await _context.InventoryStocks
                                    .Include(i => i.Product)
                                    .Include(i => i.Product.ProductSuppliers)
                                        .ThenInclude(ps => ps.Supplier)
                                    .Include(i => i.Unit)
                                    .Include(i => i.Product.Category)
                                    .Select(i => new InventoryStockDto
                                    {
                                        Id = i.Id,
                                        ProductId = i.ProductId,
                                        ProductCode = i.Product.ProductCode,
                                        ProductName = i.Product.ProductName,
                                        Category = i.Product.Category.CategoryName,
                                        UnitName = i.Unit.UnitName,
                                        Quantity = i.Quantity,
                                        ReorderLevel = i.ReorderLevel,
                                        PreferredStockLevel = i.PreferredStockLevel,
                                        IsLowstock = i.Quantity < i.ReorderLevel,
                                        Price = i.Product.Price,
                                        InventoryValue = i.Product.Price * i.Quantity,
                                        LastUpdated = i.LastUpdated,
                                        SupplierNames = i.Product.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
                                    }).AsNoTracking().ToListAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Get Stock executed in {elapsedMs} ms");
            return Ok(stocks);
        }

        [HttpGet("LowStock")]
        public async Task<ActionResult<IEnumerable<InventoryStockDto>>> GetLowStock()
        {
            var watch = new Stopwatch();
            watch.Start();
            var lowStockItems = await _context.InventoryStocks
                .Include(i => i.Product)
                .Include(i => i.Product.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(i => i.Unit)
                .Include(i => i.Product.Category)
                .Where(i => i.Quantity < i.ReorderLevel)
                .Select(i => new InventoryStockDto
                {
                    ProductId = i.ProductId,
                    ProductCode = i.Product.ProductCode,
                    ProductName = i.Product.ProductName,
                    Category = i.Product.Category.CategoryName,
                    UnitName = i.Unit.UnitName,
                    Quantity = i.Quantity,
                    ReorderLevel = i.ReorderLevel,
                    LastUpdated = i.LastUpdated,
                    IsLowstock = i.Quantity < i.ReorderLevel,
                    SupplierNames = i.Product.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
                })
                .ToListAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"GetLowStock executed in {elapsedMs} ms");
            return Ok(lowStockItems);
        }

        [HttpGet("products")]

        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {

            var products = await _context.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.Unit)
                                        .Include(p => p.ProductSuppliers)
                                            .ThenInclude(ps => ps.Supplier)
                                        .ToListAsync();
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                UnitId = p.UnitId,
                UnitName = p.Unit.UnitName,
                SupplierIds = p.ProductSuppliers.Select(ps => ps.SupplierId).ToList(),
                SupplierNames = p.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
            });

            return Ok(result);
        }

        [HttpPost("AddStock")]
        public async Task<ActionResult<InventoryStockDto>> AddStock([FromBody] InventoryStockCreateDto dto)
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

            var result = await _context.InventoryStocks
                                        .Include(i => i.Product)
                                            .ThenInclude(i => i.Category)
                                        .Include(i => i.Product.Unit)
                                        .Include(i => i.Product.ProductSuppliers)
                                            .ThenInclude(i => i.Supplier)
                                       .Where(i => i.Id == stock.Id)
                                       .Select(p => new InventoryStockDto
                                       {
                                           Id = p.Id,
                                           ProductCode = p.Product.ProductCode,
                                           ProductName = p.Product.ProductName,
                                           Category = p.Product.Category.CategoryName,
                                           UnitName = p.Product.Unit.UnitName,
                                           Quantity = p.Quantity,
                                           ReorderLevel = p.ReorderLevel,
                                           PreferredStockLevel = p.PreferredStockLevel,
                                           LastUpdated = p.LastUpdated,
                                           SupplierNames = p.Product.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
                                       }).FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetLowStock), new { id = stock.Id }, result);
        }

        [HttpPut("UpdateReorderLevel/{id}")]
        public async Task<ActionResult> UpdateReorderLevel(int id, [FromBody] InventoryStockUpdateDto dto)
        {
            var stock = await _context.InventoryStocks.FindAsync(id);
            if (stock == null) return NotFound();

            if (dto.Quantity.HasValue) stock.Quantity = dto.Quantity.Value;

            if (dto.ReorderLevel.HasValue) stock.ReorderLevel = dto.ReorderLevel.Value;

            if (dto.PreferredStockLevel.HasValue) stock.PreferredStockLevel = dto.PreferredStockLevel.Value;

            stock.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                stock.Id,
                stock.ProductId,
                stock.Quantity,
                stock.ReorderLevel,
                stock.LastUpdated,
                stock.PreferredStockLevel
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStockController(int id)
        {
            var stock = await _context.InventoryStocks.FindAsync(id);

            if (stock == null) return NoContent();

            _context.InventoryStocks.Remove(stock);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{stock.Product.ProductName} have been removed" });
        }
    }
}