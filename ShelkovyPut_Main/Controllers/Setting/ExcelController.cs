using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domain.Models.Management;
using Application.ViewModels.Setting;
using Infrastructure.Data;
using Application.Interfaces.Setting;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ShelkovyPut_Main.Controllers.Setting
{
    public class ExcelController : Controller 
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly IExcelFileImportService _excel;

        public ExcelController(ShelkobyPutDbContext context, IExcelFileImportService excel)
        {
            _context = context;
            _excel = excel;
        }

        [HttpGet("upload")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportBrands(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            using(var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                List<BrandExelImporter> brands = await _excel.ImportBrands(stream);
            }

            return Ok("Brands imported successfully");
        }

        [HttpPost]
        public async Task<IActionResult> ImportCategories(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                List<CategoryForExcelImportVM> categories = await _excel.ImportCategories(stream);
            }

            return Ok("Categories imported successfully");
        }

        [HttpPost]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                List<ProductExcelImporter> categories = await _excel.ImportProducts(stream);
            }

            return Ok("Produts imported successfully");
        }
    }
}
