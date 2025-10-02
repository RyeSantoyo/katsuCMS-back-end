using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
        {
            var units = await _context.Units.Select(u=> new UnitDto
            {
                Id = u.Id,
                UnitName = u.UnitName
            }).ToListAsync();

            if (!units.Any())
            {
                return NotFound();
            }
            return Ok(units);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<UnitDto>> GetUnit(int id)
        {
            var unit = await _context.Units
            .Where(u => u.Id == id)
            .Select(u => new UnitDto
            {
                Id = u.Id,
                UnitName = u.UnitName
            })
            .FirstOrDefaultAsync();

            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]

        public async Task<ActionResult<UnitDto>> CreateUnit(UnitCreateDto dto)
        {
            var exists = await _context.Units.FirstOrDefaultAsync(u => u.UnitName == dto.UnitName);
            if (exists != null)
            {
                return Conflict("Unit with the same ID already exists.");
            }

            var unit = new Unit
            {
                UnitName = dto.UnitName
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            var result = new UnitDto
            {
                Id = unit.Id,
                UnitName = unit.UnitName
            };

            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, UnitUpdateDto dto)
        {
            var exists = await _context.Units.FindAsync(id);
            if (exists == null) return NotFound();

            exists.UnitName = dto.UnitName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}