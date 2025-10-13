using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Display (Name ="New Password")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match")]

        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [Display (Name ="Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
