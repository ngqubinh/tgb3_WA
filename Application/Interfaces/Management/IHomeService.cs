using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface IHomeService
    {
        Task<IEnumerable<Product>> GetAllproducts(string sTerm = "", int categoryId = 0, int sizeId = 0, int page = 1, int pageSize = 12, int brandId = 0);
        Task<IEnumerable<Product>> GetFeaturedProducts(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Product>> GetProducts(string sTerm = "", int categoryId = 0, int sizeId = 0, int page = 1, int pageSize = 10, int brandId = 0);
        Task<IEnumerable<Category>> Categories();
        Task<IEnumerable<Brands>> GetBrands();
        Task<IEnumerable<Size>> GetSizes();
        Task<IEnumerable<Brands>> Brands();
        Task<IEnumerable<Product>> GetNewProducts(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Product>> GetProductsSoldThisWeek(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Product>> GetSaleProducts(string sTerm = "");
        Task<IEnumerable<Product>> GetBrandProducts(string sTerm = "");
        Task<int> GetTotalProductCount(string sterm, int categoryId);
    }
}
