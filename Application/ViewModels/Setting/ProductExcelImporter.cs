using Domain.Models.Management;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.ViewModels.Setting
{
    public class ProductExcelImporter
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int? SizeId { get; set; }
        public string? Pictures { get; set; }
        public double ProductPrice { get; set; }
        public bool IsFeatured { get; set; } = false;
        public double? DiscountProductprice { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
