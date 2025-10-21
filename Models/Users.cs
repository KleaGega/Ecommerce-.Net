using Microsoft.AspNetCore.Identity;
namespace MVCProject.Models
{
        public class Users : IdentityUser
        {
            public string? FullName { get; set; }
            public string? City { get; set; }
            public string PhoneNumber2 { get; set; } = string.Empty;
            public string? RefreshToken { get; set; }
            public DateTime RefreshTokenExpiryTime { get; set; }
            public ICollection<CartItem> CartItems { get; set; }
            public ICollection<Order> Orders { get; set; }
    }
    }



