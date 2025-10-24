namespace MVCProject.ViewModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status {  get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
