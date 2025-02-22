using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Request.Management
{
    public class CreateProductRequest
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Pictures { get; set; }
        public IEnumerable<IFormFile>? PicturePaths { get; set; }
        public double ProductPrice { get; set; }
        public bool IsFeatured { get; set; } = false;
        public double? DiscountProductprice { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int SizeId { get; set; }
    }
}
