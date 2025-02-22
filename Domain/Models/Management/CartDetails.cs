using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class CartDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [ForeignKey(nameof(ShoppingCartId))]
        public ShoppingCart? ShoppingCart { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
