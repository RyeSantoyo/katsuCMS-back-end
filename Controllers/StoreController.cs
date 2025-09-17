using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<StoreDto>>> GetStores()
        {
            var stores = await _context.Stores.Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                ContactNumber = s.ContactNumber
            }).ToListAsync();

            return Ok(stores);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<StoreDto>> GetStore(int id)
        {
            var store = await _context.Stores
             .Where(s => s.Id == id)
             .Select(s => new StoreDto
             {
                 Id = s.Id,
                 Name = s.Name,
                 Address = s.Address,
                 ContactNumber = s.ContactNumber
             }).FirstOrDefaultAsync();

            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]

        public async Task<ActionResult<StoreDto>> CreateStore(StoreCreateDto dto)
        {
            var store = new Store
            {
                Name = dto.Name,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber
            };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            var result = new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                ContactNumber = store.ContactNumber
            };

            return CreatedAtAction(nameof(GetStore), new { id = store.Id }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, StoreUpdateDto dto)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return NotFound();

            store.Name = dto.Name;
            store.Address = dto.Address;
            store.ContactNumber = dto.ContactNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}