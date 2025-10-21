namespace MVCProject.ViewModels
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string UserName { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string ImagePath { get; set; }
    }

}
