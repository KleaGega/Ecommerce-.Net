using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        public string? City { get; set; }
        public string PhoneNumber2 { get; set; } = string.Empty;
        [Required(ErrorMessage ="Password is required")]
        [StringLength(40,MinimumLength=8,ErrorMessage ="The {0} must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword",ErrorMessage ="Password does not match")]

        public string Password { get; set; }
        [Required(ErrorMessage ="Confirm password is required")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
