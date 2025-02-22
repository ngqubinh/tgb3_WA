using Application.Interfaces.Management;
using Domain.Models.Management;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class BrandController : Controller
    {
        private readonly IGenericService<Brands> _generic;

        public BrandController(IGenericService<Brands> generic)
        {
            _generic = generic;
        }

        public async Task<IActionResult> Brand()
        {
            var sizes = await _generic.GetAllAsync();
            if (sizes == null)
            {
                return new JsonResult("There is no size in database");
            }
            return View(sizes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brands brand)
        {
            if (ModelState.IsValid)
            {
                await _generic.AddAsync(brand);
                return RedirectToAction(nameof(Brand));
            }

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _generic.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound("This size id is not found");
            }

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brands brand)
        {
            if (ModelState.IsValid)
            {
                await _generic.UpdateAsync(brand);
                return RedirectToAction(nameof(Brand));
            }
            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _generic.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound("This id size is not found");
            }
            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _generic.DeleteAsync(id);
            return RedirectToAction(nameof(Brand));
        }
    }
}
