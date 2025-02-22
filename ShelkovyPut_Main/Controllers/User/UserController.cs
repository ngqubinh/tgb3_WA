using Application.Interfaces.Management;
using Application.ViewModels.Order;
using Application.ViewModels.User;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using Infrastructure.Repositories.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Security.Claims;

namespace ShelkovyPut_Main.Controllers.User
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<Domain.Models.Auth.User> _userManager;
        private readonly ShelkobyPutDbContext _context;
        private readonly IOrderService _order;
        private readonly MongoDBContext _mongoContext;
        private readonly IHomeService _home;
        public UserController(UserManager<Domain.Models.Auth.User> userManager, ShelkobyPutDbContext context, IOrderService order, MongoDBContext mongoContext, IHomeService home)
        {
            _userManager = userManager;
            _context = context;
            _order = order;
            this._mongoContext = mongoContext;
            this._home = home;
        }

        [HttpGet]
        [Route("User/Dashboard")]
        public async Task<IActionResult> Dashboard(string filter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("https://localhost:7235/identity/account/login");
            }

            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandsForSearch = await _home.GetBrands();

            var profileVM = new ProfileVM()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                BirhtDate = user.BirthDate,
                Sex = user.Sex,
                CreatedDate = user.CreatedDate,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandsForSearch
            };
            return View(profileVM);
        }

        [HttpGet]
        [Route("User/Account")]
        public async Task<IActionResult> Account()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("https://localhost:7235/identity/account/login");
            }
            var orders = await _order.UserOrders();
            return View(orders);
        }

        [HttpGet]
        [Route("User/Profile/{id}")]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("404 Profile page");
            }

            var viewModel = new ProfileVM()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                CreatedDate = user.CreatedDate,
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("User/MyOrders")]
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _order.UserOrders();
            return View(orders);
        }

        [HttpGet]
        [Route("User/Profile/Edit")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("404 Profile page");
            }

            IEnumerable<Category> categoriesForSearch = await _context.Categories.ToListAsync();
            IEnumerable<Brands> brnadsForSearch = await _context.Brands.ToListAsync();

            var profileVM = new ProfileVM()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brnadsForSearch
            };
            var viewModel = new EditProfileVM()
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
                Profile = profileVM,
            };
            viewModel.SplitBirthDate();
            return View(viewModel);
        }

        [HttpPost]
        [Route("User/Profile/Edit")]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }

            model.CombineBirthDate();
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.BirthDate = model.BirthDate;
            user.Sex = model.Sex;
            
            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded) 
            {
                return RedirectToAction(nameof(Dashboard));
            }
            foreach(var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        [HttpGet]
        [Route("User/Orders/Details/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderDetails = await _order.GetOrderDetails(id);
            if (orderDetails == null)
            {
                return NotFound("No data");
            }
            return View(orderDetails);
        }
    
        public async Task<IActionResult> Chat()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);

            var filter = Builders<OMessage>.Filter.Where(m => m.UserId == userId || userRole == StaticUserRole.ADMIN);
            var messages = await _mongoContext.Messages.Find(filter).ToListAsync();

            // Manually join with User collection
            var userIds = messages.Select(m => m.UserId).Distinct().ToList();
            var users = await _mongoContext.Users.Find(Builders<Domain.Models.Auth.User>.Filter.In(u => u.Id, userIds)).ToListAsync();
            var userDictionary = users.ToDictionary(u => u.Id, u => u);

            foreach (var message in messages)
            {
                message.User = userDictionary.GetValueOrDefault(message.UserId);
            }

            return View(messages);
        }

        [HttpGet]
        [Route("User/TrackOrder")]
        public async Task<IActionResult> TrackOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("https://localhost:7235/identity/account/login");
            }

            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandsForSearch = await _home.GetBrands();

            var profileVM = new ProfileVM()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandsForSearch,
                TrackOrder = new TrackOrderVM()
            };
            return View(profileVM);
        }
    
        [HttpPost]
[Route("User/TrackOrder")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> TrackOrder(ProfileVM model)
{
    if (!ModelState.IsValid)
    {
        // Log the validation errors for debugging
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        foreach (var error in errors)
        {
            Console.WriteLine(error); 
        }
        model.CategoryForSearch = await _home.Categories();
        model.BrandForSearch = await _home.GetBrands();
         return NotFound("Khong tim don don hang");
    }

    var orderDetails = await _order.GetOrderDetails(model.TrackOrder.OrderId);
    if (orderDetails == null)
    {
        ModelState.AddModelError(string.Empty, "No order found with the provided details.");
        model.CategoryForSearch = await _home.Categories();
        model.BrandForSearch = await _home.GetBrands();
        return NotFound("Khong tim don don hang");
    }

    return RedirectToAction(nameof(OrderDetails), new { id = model.TrackOrder.OrderId });
}

    }
} 
