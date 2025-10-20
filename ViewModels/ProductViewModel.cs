using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? ImagePath { get; set; }
        public string? CategoryName { get; set; } 
        public int? CategoryId { get; set; }
    }


}
