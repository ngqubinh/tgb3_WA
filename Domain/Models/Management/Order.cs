using Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Management
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public int OrderStatusId { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<OrderDetails?> OrderDetails { get; set; } = new List<OrderDetails?>();
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }
        [Required]
        public string? MobileNumber { get; set; }        
        [Required]
        [MaxLength(30)]
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        //public int DemoId { get; set; }
        //[ForeignKey(nameof(DemoId))]
        //public Demo? Demo { get; set; }
        public int AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Addresses { get; set; }

        public bool IsShipped { get; set; }
        public int? ShipperId { get; set; }
        public Shipper Shipper { get; set; }
    }
}
