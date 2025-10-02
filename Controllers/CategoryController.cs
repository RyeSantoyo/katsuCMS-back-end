using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PCategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
            .Select(c => new PCategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            })
            .ToListAsync();

            if (!categories.Any())
            {
                return NotFound();
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<PCategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new PCategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            })
            .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]

        public async Task<ActionResult<PCategoryDto>> CreateCategory (PCategoryCreateDto dto)
        {
            var exist = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);
            if (exist != null) return Conflict("Category with the same name already exists.");
            
            var category = new ProductCategory
            {
                CategoryName = dto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new PCategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, PCategoryCreateDto dto)
        {
            var exists = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (exists == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            exists.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();

            return Ok( new { message = $"Category {id} updated successfully." });
        }
    }

}