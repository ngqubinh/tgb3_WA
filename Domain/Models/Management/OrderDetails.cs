using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        //public double Price { get; set; }
    }
}
