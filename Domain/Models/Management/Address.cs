using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string Settlement {  get; set; } = null!;
        public string? District {  get; set; }
        public string? Province { get; set; }
        public string? City {  get; set; }

        // Relationships
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
