using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
    }

}
