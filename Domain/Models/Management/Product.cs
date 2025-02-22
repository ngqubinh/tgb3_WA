using Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Management
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int SizeId { get; set; }
        [ForeignKey(nameof(SizeId))]
        public Size? Size {  get; set; }
        public string? Pictures { get; set; }
        public double ProductPrice { get; set; }
        public bool IsFeatured { get; set; } = false;
        public double? DiscountProductprice { get; set; }
        public int BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        public Brands Brands { get; set; }
        public DateTime CreatedDate { get; set; }

        // Relationship 
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public Stock? Stock { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
    }
}
