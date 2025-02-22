using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(40)]
        public string? StatusName { get; set; }
    }
}
