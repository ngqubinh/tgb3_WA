using Application.DTOs.Request.Management;
using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task CreateCategory(CreateCategoryRequest model);
        Task EditCategory(int id, CreateCategoryRequest model);
        Task DeleteCategory(int id);
        Task<Category> GetCategoryById(int id);
    }
}
