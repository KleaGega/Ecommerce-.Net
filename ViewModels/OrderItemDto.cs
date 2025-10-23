namespace MVCProject.ViewModels
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public decimal UnitPrice { get; set; }
        public int  Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}
