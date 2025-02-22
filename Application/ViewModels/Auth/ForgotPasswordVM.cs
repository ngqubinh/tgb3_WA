using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Auth
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

    }
}
