using Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Management
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Relationship
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
