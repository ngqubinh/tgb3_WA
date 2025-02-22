using Domain.Models.Management;

namespace Application.ViewModels.Page
{
    public class HeaderVM
    {
        public ShoppingCart? Carts { get; set; }
        public IEnumerable<Category>? CategoryForSearch { get; set; }
        public IEnumerable<Brands>? BrandForSearch { get; set; }
         
    }
}