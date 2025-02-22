using Domain.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty!;
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<CartDetails> CartDetails { get; set; } = new List<CartDetails>();
    }
}
