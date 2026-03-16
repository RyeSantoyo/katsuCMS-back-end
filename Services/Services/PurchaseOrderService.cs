using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.PurchaseOrder;
using katsuCMS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Services.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<(bool Success, string Message, string poNumber)> CreatePOAsync(PurchaseOrderDto pDto)
        {
            if (pDto == null)
                return (false, "Invalid Purchase Order data.", "");
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == pDto.SupplierId);
            // var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == pDto.SupplierId && s.SupplierCode == pDto.SupplierCode);
            if (!supplierExists)
                return (false, "Supplier does not exist.", "");

            Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(pDto)}");

            if (pDto.OrderDetails == null || pDto.OrderDetails.Count == 0)
            {
                return (false, "No Order detail available", "");
            }
            try
            {
                var poNumber = await GeneratePONumberAsync();
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
                        return (false, $"Product with ID {item.ProductId} does not exist.", "");

                    var unitExists = await _context.Units.AnyAsync(u => u.Id == item.UnitId);
                    if (!unitExists)
                        return (false, $"Unit with ID {item.UnitId} does not exist.", "");

                    var validSupplierProduct = await _context.ProductSuppliers
                                                .AnyAsync(ps => ps.ProductId == item.ProductId && ps.SupplierId == pDto.SupplierId);
                    if (!validSupplierProduct)
                        return (false, $"Product {item.ProductId} is not supplied by this Supplier.", "");
                }

                await _context.PurchaseOrders.AddAsync(newPo);
                await _context.SaveChangesAsync();
                return (true, "Purchase Order Created Successfully", poNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, $"Error Occured at line 261: {ex.Message}", "");

            }
        }

        public Task<(bool Success, string Message)> DeletePOAsync(int id)
        {
            throw new NotImplementedException();
        }



        public async Task<IEnumerable<object>> GetAllPurchaseOrdersAsync()
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
            return pos;
        }


        public async Task<IEnumerable<object>> GetProductsAsync()
        {
            var products = await _context.Products.Select(p => new { p.Id, p.ProductName }).AsNoTracking().ToListAsync();
            return products;
        }

        public async Task<IEnumerable<object>> GetSupplierAsync()
        {
            var suppliers = await _context.Suppliers.Select(s => new { s.Id, s.SupplierName }).AsNoTracking().ToListAsync();
            return suppliers;
        }

        public async Task<IEnumerable<object>> GetPOByIdAsync(int id)
        {
            var po = await _context.PurchaseOrders
                                    .Include(po => po.Supplier)
                                    .Include(po => po.PurchaseOrderDetails)
                                        .ThenInclude(d => d.Product)
                                        .ThenInclude(p => p.Unit)
                                        .AsNoTracking()
                                    .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null)
            {
                return Enumerable.Empty<object>();
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
            return new[] { poDto };
        }
        public async Task<IEnumerable<object>> GetSupplierByIdAsync(int id)
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
            return productSuppliers;
        }

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
        
        public async Task<string> GeneratePONumberAsync()
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

            return $"PO-{lastNumber + 1:D4}";
        }


        public async Task<(bool Success, string Message)> UpdatePOAsync(int id, PurchaseOrderStatus newStatus)
        {
            var po = await _context.PurchaseOrders
                       .Include(p => p.PurchaseOrderDetails)
                       .ThenInclude(d => d.Product)
                       .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null)
                return (false, "Purchase Order not found.");

            if (!IsValid(po.Status, newStatus))
                return (false, $"Cannot change status from {po.Status} to {newStatus}");

            po.Status = newStatus;

            if (newStatus == PurchaseOrderStatus.Completed)
            {
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
                            Receiver = "Store Admin",
                        });

                        var stock = await _context.InventoryStocks
                            .FirstOrDefaultAsync(s => s.ProductId == detail.ProductId && s.UnitId == detail.UnitId);

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

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Error updating stock: {ex.Message}");
                }
            }
            return (true, "Status updated successfully");
        }
    }
}