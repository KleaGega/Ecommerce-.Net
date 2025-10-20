using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Migrations;
using MVCProject.Models;
using MVCProject.ViewModels;
using System.Threading.Tasks;

namespace MVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly MvcProductContext _context;
        public CategoryController(MvcProductContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Category.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Category
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("GetCategoryId")]
        public async Task<IActionResult> GetCategoryId(int id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Category
                .Where(c => c.Id == id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return Ok(category);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id )
        {
            if (id == null) return NotFound();
            var category = await _context.Category.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit (int id, [FromBody] CategoryViewModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingCategory = await _context.Category.FindAsync(id);
            if (existingCategory == null) return NotFound();
            existingCategory.Name = model.Name;
            existingCategory.Description = model.Description;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description
            });          
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Category.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            var hasProducts = await _context.Product.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
                return BadRequest(new { message = "Cannot delete category because it has associated products." });

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category deleted successfully",
                category = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            });
        }
    }
}
