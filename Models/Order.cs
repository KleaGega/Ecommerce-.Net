namespace MVCProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public List<OrderProduct> Products { get; set; }
        //public Discount Discount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
