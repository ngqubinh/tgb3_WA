using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Auth;

namespace Domain.Models.Management
{
    public class WishList 
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
        public string UserId { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}