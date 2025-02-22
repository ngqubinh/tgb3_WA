using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _category;

        public CategoryController(ICategoryService category)
        {
            _category = category;
        }

        [HttpGet]
        public async Task<IActionResult> Category()
        {
            var categories = await _category.GetAllCategories();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Category2()
        {
            var categories = await _category.GetAllCategories();
            return View(categories);
        }

        [HttpGet]        
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(CreateCategoryRequest model)
        {
            await _category.CreateCategory(model);
            return RedirectToAction(nameof(Category));
        }
    }
}
