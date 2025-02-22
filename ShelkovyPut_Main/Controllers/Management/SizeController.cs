using Application.Interfaces.Management;
using Domain.Models.Management;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class SizeController : Controller
    {
        private readonly IGenericService<Size> _generic;

        public SizeController(IGenericService<Size> generic)
        {
            _generic = generic;
        }

        [HttpGet]
        public async Task<IActionResult> Size()
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
        public async Task<IActionResult> Create(Size size)
        {
            if (ModelState.IsValid)
            {
                await _generic.AddAsync(size);
                return RedirectToAction(nameof(Size));
            }

            return View(size);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var size = await _generic.GetByIdAsync(id);
            if(size == null)
            {
                return NotFound("This size id is not found");
            }

            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Size size)
        {
            if (ModelState.IsValid)
            {
                await _generic.UpdateAsync(size);
                return RedirectToAction(nameof(Size));
            }
            return View(size);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var size = await _generic.GetByIdAsync(id);
            if (size == null)
            {
                return NotFound("This id size is not found");
            }
            return View(size);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _generic.DeleteAsync(id);
            return RedirectToAction(nameof(Size));
        }
    }
}
