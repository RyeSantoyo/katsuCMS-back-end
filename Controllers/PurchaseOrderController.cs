using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.PurchaseOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAll()
        {
            var pos = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(d => d.Product)
                .Select(po => new
                {
                    po.Id,
                    po.PONumber,
                    po.Supplier.SupplierName,
                    Productname = string.Join(", ", po.PurchaseOrderDetails.Select(od => od.Product.ProductName)),
                    po.OrderDate,
                    Status = po.Status.ToString(),
                    po.TotalAmount
                }).AsNoTracking().ToListAsync();
            Console.WriteLine($"Retrieved PO count: {pos.Count}");
            return Ok(new { message = "Purchase Orders retrieved successfully", data = pos });
        }

        [HttpGet("supplier")]

        public async Task<ActionResult> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                                        .Select(s => new { s.Id, s.SupplierName })
                                        .AsNoTracking()
                                        .ToListAsync();
            return Ok(new { message = "Suppliers retrieved successfully", data = suppliers });
        }
        [HttpGet("products")]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _context.Products.Select(p => p.ProductName)
                                        .AsNoTracking()
                                        .ToListAsync();
            return Ok(new { message = "Products retrieved successfully", data = products });
        }
        [HttpGet("productsupplier")]
        public async Task<ActionResult> GetProductBySupplier(int id)
        {
            var productSuppliers = await _context.ProductSuppliers
                                        .Where(ps => ps.SupplierId == id)
                                        .Select(ps => new
                                        {
                                            id = ps.SupplierId,
                                            ps.Product.ProductName,
                                            ps.Product.Unit.UnitName,
                                            ps.Product.UnitId,
                                            ps.Product.Price
                                        }).AsNoTracking().ToListAsync();

            return Ok(new { message = "Product Suppliers retrieved successfully", data = productSuppliers });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            Console.WriteLine($"Fetching PO with ID {id}");
            var po = await _context.PurchaseOrders
                                    .Include(po => po.Supplier)
                                    .Include(po => po.PurchaseOrderDetails)
                                        .ThenInclude(d => d.Product)
                                        .ThenInclude(p => p.Unit)
                                    .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null)
            {
                return NotFound(new { message = "Purchase Order not found" });
            }

            var poDto = new
            {
                po.Id,
                po.PONumber,
                SupplierName = po.Supplier.SupplierName,
                po.OrderDate,
                Status = po.Status.ToString(),
                po.TotalAmount,
                PurchaseOrderDetails = po.PurchaseOrderDetails.Select(d => new
                {
                    d.Id,
                    ProductName = d.Product.ProductName,
                    UnitName = d.Product.Unit.UnitName,
                    d.Quantity,
                    d.UnitPrice,
                    d.TotalPrice
                }).ToList()
            };
            Console.WriteLine("Purchase Order retrieved successfully" + poDto);

            return Ok(new { message = "Purchase Order retrieved successfully", data = poDto });
        }
        #endregion
        #region Post
        [HttpPost]
        public async Task<IActionResult> CreatePO([FromBody] PurchaseOrderDto pDto)
        {
            if (pDto == null) return BadRequest(new { message = "Invalid input: Request body is null" });

            Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(pDto)}");

            if (pDto.OrderDetails == null || pDto.OrderDetails.Count == 0)
            {
                return BadRequest(new { message = "No Order detail available" });
            }
            try
            {
                var newPo = new PurchaseOrder
                {
                    PONumber = pDto.PONumber,
                    SupplierId = pDto.SupplierId,
                    OrderDate = pDto.OrderDate,
                    Status = pDto.Status,
                    TotalAmount = pDto.OrderDetails.Sum(d => d.Quantity * d.UnitPrice),
                    PurchaseOrderDetails = pDto.OrderDetails.Select(d => new PurchaseOrderDetail
                    {
                        PurchaseOrderNumber = d.PONumber,
                        ProductName = d.ProductName,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.Quantity * d.UnitPrice,
                        UnitId = d.UnitId
                    }).ToList()
                };
                await _context.PurchaseOrders.AddAsync(newPo);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Purchase Order Created" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Occured", error = ex.Message });

            }
        }
        #endregion
        #region Patch
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid input: Request body is null" });

            var po = await _context.PurchaseOrders
                .Include(p => p.PurchaseOrderDetails)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null) return NotFound(new { message = "Purchase Order not found." });

            if (Enum.TryParse<PurchaseOrderStatus>(dto.Status.ToString() ?? string.Empty, true, out var newStatus))
            {
                po.Status = newStatus;
                if (newStatus == PurchaseOrderStatus.Received)
                {
                    foreach (var detail in po.PurchaseOrderDetails)
                    {
                        //detail.InventoryStock.Quantity += detail.Quantity;

                        var log = new StockLogs
                        {
                            ProductId = detail.ProductId,
                            QuantityChange = (int)detail.Quantity,
                            UnitPrice = (decimal)detail.UnitPrice,
                            Reason = "Stock Received",
                            DateLogged = DateTime.Now
                        };
                        await _context.StockLogs.AddAsync(log);
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(new { message = "Status updated successfully" });
            }
            return BadRequest(new { message = "Invalid status value." });
        }
        #endregion
        #region Delete
        [HttpDelete]
        public async Task<IActionResult> DeletePo(int id)
        {
            var po = _context.PurchaseOrders
            .Include(po => po.PurchaseOrderDetails)
            .FirstOrDefault(po => po.Id == id);

            if (po == null) return BadRequest("Cannot delete this entry");

            _context.PurchaseOrderDetails.RemoveRange(po.PurchaseOrderDetails);
            _context.PurchaseOrders.Remove(po);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entry succesfully deleted" });
        }
    }
    #endregion
}