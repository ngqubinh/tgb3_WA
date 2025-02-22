using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Auth;

namespace Domain.Models.Management
{
    public class Shipper
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public DateTime ShippedDate { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}