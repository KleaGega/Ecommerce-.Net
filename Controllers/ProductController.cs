using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVCProject.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly MvcProductContext _context;

        public ProductController(MvcProductContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Product.ToListAsync();
            return Ok(products);
        }

        [HttpGet("Products")]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Product
                .Include(p => p.Category) 
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.Status,
                    p.ImagePath,
                    CategoryId = p.Category != null ? p.Category.Id : (int?)null,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    CategoryDescription = p.Category != null ? p.Category.Description : null
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.Status,
                    p.ImagePath,
                    CategoryId = p.Category.Id,
                    CategoryName = p.Category.Name,
                    CategoryDescription = p.Category.Description
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
          return Ok();
        }
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductViewModel model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                // If invalid, repopulate dropdown
                var categories = await _context.Category
                    .Select(c => new { c.Id, c.Name })
                    .ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Status = model.Status,
                CategoryId = model.CategoryId,
            };

            // Handle file upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/images/" + uniqueFileName;
            }

            // Save to database
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize]
        // GET: Product/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
                return BadRequest("Product ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = await _context.Product.FindAsync(id);
            if (existingProduct == null)
                return NotFound("Product not found.");

            try { 
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Status = product.Status;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    existingProduct.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    product = existingProduct
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                    return NotFound("Product no longer exists.");
                else
                    throw;
            }
        }

        [Authorize]
        // GET: Product/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: Product/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return Ok(product);
        }
     
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
