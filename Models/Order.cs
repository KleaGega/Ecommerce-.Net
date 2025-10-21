namespace MVCProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public Users User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
    }


}
