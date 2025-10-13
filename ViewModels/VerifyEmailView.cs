using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class VerifyEmailView
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
