using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Repositories.Management
{
    public class CategoryService : ICategoryService
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly IHttpContextAccessor _http;

        public CategoryService(ShelkobyPutDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task CreateCategory(CreateCategoryRequest model)
        {
            try
            {
                var currentUserId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName!.Equals(model.CategoryName));
                if (category != null)
                {
                    throw new Exception("Category have been created");
                }

                var newCategory = new Category()
                {
                    CategoryName = model.CategoryName,    
                    CreatedDate = model.CreatedDate,
                    UserId = currentUserId
                };

                var result = await _context.Categories.AddAsync(newCategory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }

        public Task DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditCategory(int id, CreateCategoryRequest model)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.Include(c => c.User).ToListAsync();
        }

        public Task<Category> GetCategoryById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
