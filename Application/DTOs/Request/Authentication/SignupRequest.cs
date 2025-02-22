using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Authentication
{
    public class SignupRequest : LoginRequest
    {
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
