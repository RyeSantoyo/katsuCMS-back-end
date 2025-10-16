using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
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

            // var products = await _context.Products.Select(p => new ProductDto
            // {
            //     Id = p.Id,
            //     ProductCode = p.ProductCode,
            //     ProductName = p.ProductName,
            //     UnitName = p.Unit.UnitName,
            //     SupplierName = p.Supplier.SupplierName,
            //     CategoryName = p.Category.CategoryName,
            //     UnitId = p.UnitId,
            //     SupplierId = p.SupplierId,
            //     CategoryId = p.CategoryId,
            //     Quantity = p.Quantity,
            //     Price = p.Price,
            //     Description = p.Description
            // }).ToListAsync();
            // if (!products.Any())
            // {
            //     return NotFound();
            // }
            // return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            // var product = await _context.Products.Where(p => p.Id == id)
            // .Select(p => new ProductDto
            // {
            //     Id = p.Id,
            //     ProductCode = p.ProductCode,
            //     ProductName = p.ProductName,
            //     UnitName = p.Unit.UnitName,
            //     SupplierName = p.Supplier.SupplierName,
            //     CategoryName = p.Category.CategoryName,
            //     UnitId = p.UnitId,
            //     SupplierId = p.SupplierId,
            //     CategoryId = p.CategoryId,
            //     Quantity = p.Quantity,
            //     Price = p.Price,
            //     Description = p.Description
            // }).FirstOrDefaultAsync();

            // if (product == null)
            // {
            //     return NotFound();
            // }
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .Include(p => p.Unit)
                                         .Include(p => p.ProductSuppliers)
                                            .ThenInclude(ps => ps.Supplier)
                                         .FirstOrDefaultAsync(p => p.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            var result = new ProductDto
            {
                Id = products.Id,
                ProductCode = products.ProductCode,
                ProductName = products.ProductName,
                Price = products.Price,
                Description = products.Description,
                CategoryId = products.CategoryId,
                CategoryName = products.Category.CategoryName,
                UnitId = products.UnitId,
                UnitName = products.Unit.UnitName,
                SupplierIds = products.ProductSuppliers.Select(ps => ps.SupplierId).ToList(),
                SupplierNames = products.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var product = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                UnitId = dto.UnitId,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                Description = dto.Description,
                ProductSuppliers = dto.SupplierIds.Select(supplierId => new ProductSupplier
                {
                    SupplierId = supplierId
                }).ToList()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var result = await _context.Products.Where(p => p.Id == product.Id)
            .Select(p => new ProductDto
            {
                Id = product.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                UnitName = p.Unit.UnitName,
                // SupplierName = p.Supplier.SupplierName,
                CategoryName = p.Category.CategoryName,
                UnitId = p.UnitId,
                // SupplierId = p.SupplierId,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Description = p.Description,
                SupplierIds = p.ProductSuppliers.Select(ps => ps.SupplierId).ToList(),
                SupplierNames = p.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
            }).FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, result);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.Include(p => p.ProductSuppliers)
                                                    .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            product.ProductCode = dto.ProductCode;
            product.ProductName = dto.ProductName;
            product.UnitId = dto.UnitId;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;
            product.Description = dto.Description;

            if (dto.SupplierIds != null)
            {
                _context.ProductSuppliers.RemoveRange(product.ProductSuppliers);

                product.ProductSuppliers = dto.SupplierIds
                    .Select(supplierId => new ProductSupplier
                    {
                        SupplierId = supplierId,
                        ProductId = product.Id
                    }).ToList();
            }

            await _context.SaveChangesAsync();

            var result = await _context.Products.Where(p => p.Id == product.Id)
            .Select(p => new ProductDto
            {
                Id = product.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                UnitName = p.Unit.UnitName,
                CategoryName = p.Category.CategoryName,
                UnitId = p.UnitId,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Description = p.Description,
                SupplierIds = p.ProductSuppliers.Select(ps => ps.SupplierId).ToList(),
                SupplierNames = p.ProductSuppliers.Select(ps => ps.Supplier.SupplierName).ToList()
            }).FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
    
