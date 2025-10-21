namespace MVCProject.ViewModels
{
    public class UpdateQuantityRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
