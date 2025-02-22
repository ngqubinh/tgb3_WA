using Domain.Models.Management;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.ViewModels
{
    public class SpecificProduct
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Pictures { get; set; }
        public double ProductPrice { get; set; }
        public bool IsFeatured { get; set; } = false;
        public double? DiscountProductprice { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserId { get; set; }
        public int CategoryId { get; set; }
        public Stock? Stock { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
    }
}
