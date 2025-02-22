using Application.DTOs.Request.Management;
using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();        
        Task<IEnumerable<Product>> GetAllRelatedProducts(int productId, int count);
        Task CreateProduct(CreateProductRequest model);
        Task EditProduct(int id, CreateProductRequest model);
        Task DeleteProduct(int id);
        Task<Product> GetProductById(int id);        
    }
}
