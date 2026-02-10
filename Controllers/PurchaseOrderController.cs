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
                    po.TotalAmount,
                    Quantity = po.PurchaseOrderDetails.Sum(d => d.Quantity),
                    ItemsCount = po.PurchaseOrderDetails.Count
                }).AsNoTracking().ToListAsync();
            Console.WriteLine($"Retrieved PO count: {pos.Count}");
            return Ok(new { message = "Purchase Orders retrieved successfully", data = pos });
        }

        [HttpGet("supplier")]

        public async Task<ActionResult> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                                        .Select(s => new { s.Id, s.SupplierName, s.SupplierCode })
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
                    ps.ProductId,
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
                                        .AsNoTracking()
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

        [HttpGet("GeneratePONumber")]
        public async Task<IActionResult> GetNextPONumber()
        {
            var poNumber = await GeneratePONumber();
            return Ok(new { poNumber });
        }
        #endregion

        #region Post
        private async Task<string> GeneratePONumber()
        {
            var lastPo = await _context.PurchaseOrders
                .OrderByDescending(po => po.Id)
                .FirstOrDefaultAsync();

            int lastNumber = 0;

            if (lastPo != null)
            {
                var parts = lastPo.PONumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out var num))
                {
                    lastNumber = num;
                }
            }

            return $"PO-{(lastNumber + 1):D4}";
        }
        [HttpPost]
        public async Task<IActionResult> CreatePO([FromBody] PurchaseOrderDto pDto)
        {
            if (pDto == null) return BadRequest(new { message = "Invalid input: Request body is null" });
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == pDto.SupplierId);
            // var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == pDto.SupplierId && s.SupplierCode == pDto.SupplierCode);
            if (!supplierExists)
                return BadRequest(new { message = "Supplier does not exist." });

            Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(pDto)}");

            if (pDto.OrderDetails == null || pDto.OrderDetails.Count == 0)
            {
                return BadRequest(new { message = "No Order detail available" });
            }
            try
            {
                var poNumber = await GeneratePONumber();
                var newPo = new PurchaseOrder
                {
                    PONumber = poNumber,
                    SupplierId = pDto.SupplierId,
                    OrderDate = pDto.OrderDate,
                    Status = pDto.Status,
                    TotalAmount = pDto.OrderDetails.Sum(d => d.Quantity * d.UnitPrice),
                    PurchaseOrderDetails = pDto.OrderDetails.Select(d => new PurchaseOrderDetail
                    {
                        PurchaseOrderNumber = poNumber,
                        ProductName = d.ProductName,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.Quantity * d.UnitPrice,
                        UnitId = d.UnitId
                    }).ToList()

                };
                Console.WriteLine(JsonSerializer.Serialize(pDto));
                foreach (var item in pDto.OrderDetails)
                {
                    var productExists = await _context.Products.AnyAsync(p => p.Id == item.ProductId);
                    if (!productExists)
                        return BadRequest(new { message = $"Product with ID {item.ProductId} does not exist." });

                    var unitExists = await _context.Units.AnyAsync(u => u.Id == item.UnitId);
                    if (!unitExists)
                        return BadRequest(new { message = $"Unit with ID {item.UnitId} does not exist." });

                    var validSupplierProduct = await _context.ProductSuppliers
                                                .AnyAsync(ps => ps.ProductId == item.ProductId && ps.SupplierId == pDto.SupplierId);
                    if (!validSupplierProduct)
                        return BadRequest(new { message = $"Product {item.ProductId} is not supplied by this Supplier." });
                }

                await _context.PurchaseOrders.AddAsync(newPo);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Purchase Order Created Successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Error Occured at line 261", error = ex.Message });

            }
        }
        #endregion
        #region Patch

        private bool IsValid(PurchaseOrderStatus current, PurchaseOrderStatus next)
        {
            return current switch
            {
                PurchaseOrderStatus.Pending => next == PurchaseOrderStatus.Approved || next == PurchaseOrderStatus.Cancelled,
                PurchaseOrderStatus.Approved => next == PurchaseOrderStatus.Completed || next == PurchaseOrderStatus.Cancelled,
                PurchaseOrderStatus.Completed => next == PurchaseOrderStatus.Completed,
                _ => false
            };

        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid input: Request body is null" });

            var po = await _context.PurchaseOrders
                .Include(p => p.PurchaseOrderDetails)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null) return NotFound(new { message = "Purchase Order not found." });

            if (!Enum.TryParse<PurchaseOrderStatus>(dto.Status.ToString() ?? string.Empty, true, out var newStatus))
                return BadRequest(new { message = "Invalid status value." });

            if (!IsValid(po.Status, newStatus))
                return BadRequest(new { message = $"Cannot change status from {po.Status} to {newStatus}" });

            po.Status = newStatus;

            if (newStatus == PurchaseOrderStatus.Completed)
            {
                // if (po.Status == PurchaseOrderStatus.Completed)
                //     return BadRequest(new { message = "PO already completed." });

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var detail in po.PurchaseOrderDetails)
                    {
                        _context.StockLogs.Add(new StockLogs
                        {
                            ProductId = detail.ProductId,
                            QuantityChange = (int)detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            Reason = "Stock Received",
                            DateLogged = DateTime.UtcNow,
                        });


                        var stock = await _context.InventoryStocks
                            .FirstOrDefaultAsync
                            (s => s.ProductId == detail.ProductId
                            && s.UnitId == detail.UnitId);

                        if (stock == null)
                        {
                            _context.InventoryStocks.Add(new InventoryStock
                            {
                                ProductId = detail.ProductId,
                                UnitId = detail.UnitId,
                                Quantity = detail.Quantity
                            });
                        }
                        else
                        {
                            stock.Quantity += detail.Quantity;
                        }
                    }
                    po.Status = newStatus;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                    return StatusCode(500, new { message = "Error Occured while updating stock", error = ex.Message });
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Status updated successfully" });
        }
        #endregion
        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePo(int id)
        {
            var po = _context.PurchaseOrders
            .Include(po => po.PurchaseOrderDetails)
            .FirstOrDefault(po => po.Id == id);

            if (po == null) return BadRequest("Cannot delete this entry");
            if (po.Status != PurchaseOrderStatus.Pending)
            {
                return BadRequest(new { message = "Cannot delete Purchase Order unless it is Pending." });
            }

            _context.PurchaseOrderDetails.RemoveRange(po.PurchaseOrderDetails);
            _context.PurchaseOrders.Remove(po);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entry succesfully deleted" });
        }
    }
    #endregion
}