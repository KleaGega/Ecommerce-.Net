using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.ViewModels;

namespace MVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MvcProductContext _context;

        public CartController(MvcProductContext context)
        {
            _context = context;
        }

        [HttpGet("UserCart/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart(string userId)
        {
            var cart = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity,
                    UserName = c.User.FullName,
                    ImagePath = c.Product.ImagePath,

                }).ToListAsync();

            return Ok(cart);
        }

        [HttpGet("UserCartLength/{userId}")]
        public async Task<ActionResult<int>> GetCartLength(string userId)
        {
            var count = await _context.CartItems
                .Where(c => c.UserId == userId)
                .CountAsync();

            return Ok(count);
        }


        [HttpPost("{userId}")]
        public async Task<IActionResult>AddToCart(string userId, [FromBody]CartAddDto dto)
        {
            var product = await _context.Product.FindAsync(dto.ProductId);
            if (product == null) return NotFound("Product not found");
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

            if (existingItem != null)
            { 
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UserId = userId
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Item added successfully",
                cart = new CartAddDto
                {
                  ProductId = product.Id,
                  Quantity = dto.Quantity,
                }
            });

        }
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is missing." });

            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest(new { message = "UserId is required." });

            if (_context?.CartItems == null)
                return StatusCode(500, new { message = "CartItems DB set is null." });

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ProductId == request.ProductId);

            if (item == null)
                return NotFound(new { message = "Cart item not found." });

            if (request.Quantity <= 0)
                _context.CartItems.Remove(item);
            else
                item.Quantity = request.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Quantity updated successfully." });
        }


        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.Include(c => c.Product).Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == cartItemId);
            if (cartItem == null) return NotFound(cartItemId);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Item deleted successfully",
                cartItem = new CartItemDto
                {
                   Id = cartItemId,
                   Quantity = cartItem.Quantity,
                   ProductName = cartItem.Product.Name,
                   ProductId = cartItem.ProductId,
                   UserName = cartItem.User.FullName,
                }
            });

        }
        [HttpDelete("remove/{userId}/{productId}")]
        public async Task<IActionResult> RemoveItem(string userId, int productId)
        {
            var item = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (item == null)
                return NotFound(new { message = "Cart item not found." });

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Item deleted successfully",
                cartItem = new CartItemDto
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    ProductName = item.Product?.Name ?? "Unknown Product",
                    ProductId = item.ProductId,
                    UserName = item.User?.FullName ?? "Unknown User",
                }
            });
        }

    }
}
