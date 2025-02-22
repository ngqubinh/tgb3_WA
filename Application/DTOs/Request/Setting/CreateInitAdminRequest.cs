using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Setting
{
    public class CreateInitAdminRequest
    {
        public string? FullName { get; set; }
        public DateOnly? BirthDate { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
