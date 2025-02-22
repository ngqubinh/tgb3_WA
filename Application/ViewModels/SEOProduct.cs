using Application.Interfaces.Management;
using Domain.Models.Management;

namespace Application.ViewModels
{
    public class SEOProduct
    {
        public Product? Product { get; set; } 
        public Size? Size { get; set; }
        public Stock? Stock { get; }
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Product>? RelatedProducts { get; set; }
        public IEnumerable<Product>? NewProducts { get; set; }
        public IEnumerable<Brands>? BrandForSearch { get; set; }
        public IEnumerable<Brands>? GetBrands { get; set; }
        public IEnumerable<Product>? FeaturedProducts { get; set; }     
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Size>? Sizes { get; set; }
        public IEnumerable<Size>? GetSizes {  get; set; }
        public IEnumerable<Category>? CategoryForSearch { get; set; }
        public IEnumerable<Product>? ProductsSoldThisWeek { get; set; }
        public IEnumerable<Product>? ProductsSale { get; set; }
        public IEnumerable<Product>? ProductsBrand { get; set; }
        public IEnumerable<WishList>? wishLists { get; set; }
        public string STerm { get; set; } = "";
        public int CategoryId { get; set; } = 0;
        public int? SizeId { get; set; }
        public int? BrandId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public double ProductPrice {  get { return Product?.ProductPrice ?? 0; } }
        public double DiscountedProductPrice { get { return Product?.DiscountProductprice ?? 0; } }
        public double DiscountedProductPricePercentage
        {
            get { return (int)Math.Ceiling(CalculateDiscountedPricePercentage(ProductPrice, DiscountedProductPrice)); }
        }

        #region
        private double CalculateDiscountedPricePercentage(double productPrice, double discountedProductPrice)
        {
            if(productPrice < 0)
            {
                throw new ArgumentException("Gia san pham phai lon hon hoac bang 0");
            }
            var result = ((productPrice - discountedProductPrice) / productPrice) * 100;
            return result;
        }
        #endregion
    }
}
