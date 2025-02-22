using Application.Interfaces.Management;
using Application.Interfaces.Setting;
using Application.ViewModels;
using Domain.Models.Management;
using Infrastructure.Repositories.Setting;
using Microsoft.AspNetCore.Mvc;
using ShelkovyPut_Main.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ShelkovyPut_Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _home;
        private readonly ISenderEmailService _emailService;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IHomeService home, ISenderEmailService senderEmail, IConfiguration config)
        {
            _logger = logger;
            _home = home;
            _emailService = senderEmail;
            _config = config;
        }

        public async Task<IActionResult> Index(string sTerm = "", int categoryId = 0 )
        {
            IEnumerable<Product> products = await _home.GetFeaturedProducts();
            IEnumerable<Product> newProducts = await _home.GetNewProducts();
            IEnumerable<Product> productsSoldThisWeek = await _home.GetProductsSoldThisWeek();
            IEnumerable<Product> saleProducts = await _home.GetSaleProducts();
            IEnumerable<Product> brandProducts = await _home.GetBrandProducts();
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandforSearch = await _home.GetBrands();

            //if (!products.Any())
            //{
            //    return NotFound("No products found");
            //}

            //if (!brandProducts.Any())
            //{
            //    return NotFound("No products found");
            //}

            //if (!newProducts.Any())
            //{
            //    return NotFound("No new products found");
            //}

            //if (!saleProducts.Any())
            //{
            //    return NotFound("No quan products found");
            //}

            var viewModel = new SEOProduct()
            {
                FeaturedProducts = products,
                STerm = sTerm,
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                NewProducts = newProducts,
                ProductsSoldThisWeek = productsSoldThisWeek, 
                BrandForSearch = brandforSearch,
                ProductsSale = saleProducts,
                ProductsBrand = brandProducts,
            };

            foreach (var rp in viewModel.FeaturedProducts)
            {
                Console.WriteLine($"Featured Product: {rp.ProductName}");
            }

            foreach (var rp in viewModel.FeaturedProducts)
            {
                Console.WriteLine($"New Product: {rp.ProductName}");
            }

            foreach (var rp in viewModel.ProductsBrand)
            {
                Console.WriteLine($"New Product: {rp.ProductName}");
            }

            return viewModel == null ? NotFound() : View(viewModel); 
        }

        [HttpGet]
        [Route("Home/Contract")]
        public async Task<IActionResult> Contract(int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandforSearch = await _home.GetBrands();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandforSearch,
            };
            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Contact model)
        {
            if (ModelState.IsValid)
            {
                var adminEmail = _config["EmailSettings:ToEmail"];
                var subject = $"New Contact Form Submission: {model.Subject}";
                var body = $"Name: {model.Name}\nPhone: {model.Phone}\nEmail: {model.Email}\nSubject: {model.Subject}\n\nMessage:\n{model.Message}";
                await _emailService.SendEmailAsync(adminEmail, subject, body);

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpGet]
        [Route("Home/AboutUs")]
        public async Task<IActionResult> AboutUs(int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandforSearch = await _home.GetBrands();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandforSearch,
            };
            return viewModel == null ? NotFound() : View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
