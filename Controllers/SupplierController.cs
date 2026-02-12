using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.Suppliers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var supplier = await _context.Suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                Address = s.Address,
                ContactNumber = s.ContactNumber,
                SupplierCode = s.SupplierCode
            }).AsNoTracking().ToListAsync();

            return Ok(supplier);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
            .Where(s => s.Id == id)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                Address = s.Address,
                ContactNumber = s.ContactNumber,
                SupplierCode = s.SupplierCode
            })
            .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> CreateSupplier(SupplierCreateDto dto)
        {
            var supplier = new Supplier
            {
                SupplierName = dto.SupplierName,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber,
                SupplierCode = dto.SupplierCode
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            var result = new SupplierDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                Address = supplier.Address,
                ContactNumber = supplier.ContactNumber,
                SupplierCode = supplier.SupplierCode
            };

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierCreateDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            supplier.SupplierName = dto.SupplierName;
            supplier.Address = dto.Address;
            supplier.ContactNumber = dto.ContactNumber;
            supplier.SupplierCode = dto.SupplierCode;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}