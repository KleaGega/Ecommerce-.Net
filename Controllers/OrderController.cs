using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.ViewModels;

namespace MVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MvcProductContext _context;

        public OrderController(MvcProductContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == dto.UserId)
            .ToListAsync();

                    if (!cartItems.Any())
                    {
                        return NotFound("Cart is empty for this user.");
            }


            var order = new Order
            {
                UserId = dto.UserId,               
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            //_context.CartItems.RemoveRange(cartItems);
            //await _context.SaveChangesAsync();

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId, 
               
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItems.Select(oi =>
                {
                    var product = cartItems.FirstOrDefault(c => c.ProductId == oi.ProductId)?.Product;
                    return new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = product?.Name,
                        ImagePath = product.ImagePath,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity
                    };
                }).ToList()

            };
            await _context.SaveChangesAsync();

            return Ok(orderDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).Include(o => o.User)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId, 
                UserName = o.User?.FullName,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    ImagePath = oi.Product.ImagePath,
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUSerId(string userId)
        {
            var orders = await _context.Orders.Where(o => o.UserId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();
            if(!orders.Any())
            {
                return NotFound("No orders found for this user");
            }
            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,

                }).ToList(),
            }).ToList();
            return Ok(ordersDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) {
                return NotFound();
             }

            var orderDto = new OrderDto 
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };
            return Ok(orderDto);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            var order = await _context.Orders
             .Include(o => o.OrderItems)
             .ThenInclude(oi => oi.Product) 
             .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };
            return Ok(orderDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            _context.Orders.Remove(order); 
            await _context.SaveChangesAsync();
            return Ok("Order deleted succesfully");
        }

    }
}
