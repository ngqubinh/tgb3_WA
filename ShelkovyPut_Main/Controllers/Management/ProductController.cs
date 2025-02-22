using System.Net.Mime;
using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Application.ViewModels;
using Domain.Constants;
using Domain.Models.Management;
using Infrastructure.Repositories.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class ProductController : Controller
    {
        private readonly IProductService _product;
        private readonly ICategoryService _category;
        private readonly IHomeService _home;
        public ProductController(IProductService product, ICategoryService category, IHomeService home)
        {
            _product = product;
            _category = category;
            _home = home;
        }

        [Authorize(Roles = StaticUserRole.ADMIN)]
        public async Task<IActionResult> Product(string brand)
        {
            var products = await _product.GetAllProducts();
            return View(products);
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts() 
        {
            var products = await _product.GetAllProducts();           
            return Json(products);
        }

        [HttpGet("ProductForUser")]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Text.Html)]
        public async Task<IActionResult> ProductForUser(string sterm = "", int categoryId = 0, int sizeId=0, int page = 1, int pageSize = 12, int brandId=0)
        {
            IEnumerable<Product> products;

            if (string.IsNullOrWhiteSpace(sterm) && categoryId == 0 && sizeId == 0 && brandId == 0)
            {
                products = await _home.GetAllproducts(sterm, categoryId, sizeId, page, pageSize, brandId);
            }
            else
            {
                products = await _home.GetProducts(sterm, categoryId, sizeId, page, pageSize, brandId);
            }

            if (!products.Any())
            {
                return NotFound("No products found");
            }

            IEnumerable<Category> categories = await _home.Categories();
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Size> getSizes = await _home.GetSizes();
            IEnumerable<Size> sizes = await _home.GetSizes();
            IEnumerable<Brands> brands = await _home.GetBrands();
            

            var viewModel = new SEOProduct()
            {
                Products = products,
                Categories = categories.Where(c => products.Any(p => p.CategoryId == c.Id)),
                CategoryForSearch = categoriesForSearch,
                STerm = sterm,
                CategoryId = categoryId,
                SizeId = sizeId,
                Sizes = sizes.Where(s => products.Any(p => p.SizeId == s.Id)),
                GetSizes = getSizes,
                BrandId = brandId,
                BrandForSearch = brands,
                Page = page,
                PageSize = pageSize,
                TotalCount = await _home.GetTotalProductCount(sterm, categoryId)
            };

            // if(Request.Headers["Accept"].ToString().Contains("application/json"))
            // {
            //     return Json(viewModel);
            // }

            var acceptHeader = Request.Headers[HeaderNames.Accept].ToString();
            if(acceptHeader.Contains(MediaTypeNames.Application.Json))
            {
                return Json(viewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ProductDetail(int id, string sterm = "", int categoryId = 0)
        {
            var productResponse = await _product.GetProductById(id);
            if (productResponse == null)
            {
                return NotFound($"Khong co san pham {productResponse}");
            }

            if (categoryId == 0)
            {
                categoryId = productResponse.CategoryId;
            }

            IEnumerable<Product> products = await _home.GetProducts(sterm, categoryId);
            IEnumerable<Category> categories = await _home.Categories();
            IEnumerable<Category> categoriesForSearch = await _home.Categories();

            var relatedProducts = await _product.GetAllRelatedProducts(productResponse.Id, 5);
            var viewModel = new SEOProduct()
            {
                Product = productResponse,
                Products = products,                
                RelatedProducts = relatedProducts,
                CategoryForSearch = categoriesForSearch,
                Categories = categories,
                STerm = sterm,
                CategoryId = categoryId
            };

            Console.WriteLine($"Product: {viewModel.Product.ProductName}");
            foreach (var rp in viewModel.RelatedProducts)
            {
                Console.WriteLine($"Related Product: {rp.ProductName}");
            }

            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _category.GetAllCategories();
            ViewData["Categories"] = new SelectList(categories, "Id", "CategoryName");
            var brands = await _home.Brands();
            ViewData["Brands"] = new SelectList(brands, "Id", "BrandName");
            var sizes = await _home.GetSizes();
            ViewData["Sizes"] = new SelectList(sizes, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductRequest model)
        {
            var categories = await _category.GetAllCategories();
            ViewData["Categories"] = new SelectList(categories, "Id", "CategoryName");
            var brands = await _home.Brands();
            ViewData["Brands"] = new SelectList(brands, "Id", "BrandName");
            var sizes = await _home.GetSizes();
            ViewData["Sizes"] = new SelectList(sizes, "Id", "Name");
            await _product.CreateProduct(model);
            return RedirectToAction(nameof(Product));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _product.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new CreateProductRequest
            {
                Id = product.Id,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                Description = product.Description,
                IsFeatured = product.IsFeatured,
                DiscountProductprice = product.DiscountProductprice,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                SizeId = product.SizeId,
                Pictures = product.Pictures,
            };

            ViewBag.Categories = new SelectList(await _home.Categories(), "Id", "CategoryName"); 
            ViewBag.Brands = new SelectList(await _home.GetBrands(), "Id", "BrandName"); 
            ViewBag.Sizes = new SelectList(await _home.GetSizes(), "Id", "Name"); 

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editproduct(int id, CreateProductRequest model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _product.EditProduct(id, model);
                    return RedirectToAction(nameof(Product)); 
                }
                catch
                {
                    return StatusCode(500, "Internal server error");
                }
            }

            ViewBag.Categories = new SelectList(await _home.Categories(), "Id", "CategoryName");
            ViewBag.Brands = new SelectList(await _home.GetBrands(), "Id", "BrandName");
            ViewBag.Sizes = new SelectList(await _home.GetSizes(), "Id", "Name");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _product.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _product.DeleteProduct(id);
            return RedirectToAction(nameof(Product));
        }
    }
}
