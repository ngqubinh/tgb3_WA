using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Auth
{
    public class RefreshTokens
    {
        [Key]
		public Guid ID { get; set; }
		[Required]
		[StringLength(100)]
		public string? Token { get; set; }
		public DateTime DateCreatedUtc { get; set; } = DateTime.UtcNow;
		public DateTime DateExpiresUtc { get; set; }
		public bool IsExpired => DateTime.UtcNow >= DateExpiresUtc;

		// Relationship 
		[Required]
		public string? UserID { get; set; }

		[ForeignKey("UserID")]
		public User? User { get; set; }
    }
}