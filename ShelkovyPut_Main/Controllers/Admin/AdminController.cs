using Application.Interfaces.Management;
using Application.ViewModels.Order;
using Application.ViewModels.User;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ShelkovyPut_Main.Controllers.Admin
{
    [Authorize(Roles = StaticUserRole.ADMIN + "," +StaticUserRole.STAFF)]
    public class AdminController : Controller
    {
        private readonly IOrderService _order;
        private readonly UserManager<Domain.Models.Auth.User> _userManager;
        private readonly ShelkobyPutDbContext _context;
        private readonly MongoDBContext _mongoContext;
        public AdminController(IOrderService order, UserManager<Domain.Models.Auth.User> userManager, ShelkobyPutDbContext context, MongoDBContext mongoContext)
        {
            _order = order;
            _userManager = userManager;
            _context = context;
            this._mongoContext = mongoContext;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _order.AdminOrders(true);
            return View(orders);
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _order.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                // log exception here
            }
            return RedirectToAction(nameof(AllOrders));
        }

        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _order.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id:{orderId} does not found.");
            }
            var orderStatusList = (await _order.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = order.OrderStatusId == orderStatus.Id };
            });
            var data = new UpdateOrderStatusVM
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusVM data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    data.OrderStatusList = (await _order.GetOrderStatuses()).Select(orderStatus =>
                    {
                        return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = orderStatus.Id == data.OrderStatusId };
                    });

                    return View(data);
                }
                await _order.ChangeOrderStatus(data);
                TempData["msg"] = "Updated successfully";
            }
            catch (Exception ex)
            {
                // catch exception here
                TempData["msg"] = "Something went wrong";
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> AssignOrders()
        {
            var shippers = await _userManager.GetUsersInRoleAsync(StaticUserRole.STAFF);
            var orders = await _order.GetUnshippedOrdersAsync();
            orders.ForEach(o => o.AvailableShippers = shippers.ToList());
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> AssignOrders(List<UnshippedOrderVM> model)
        {
            await _order.AssignShipperToOrdersAsync(model);
            return RedirectToAction(nameof(AssignOrders));
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("https://localhost:7235/identity/account/login");
            }

            var profileVM = new ProfileVM()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                BirhtDate = user.BirthDate,
                Sex = user.Sex,
                CreatedDate = user.CreatedDate,
            };
            return View(profileVM);
        }
    
        public async Task<IActionResult> Chat()
        {
            var messages = await _mongoContext.Messages.Find(_ => true).ToListAsync();
            var distinctCustomers = messages.Select(m => m.User)
                .DistinctBy(u => u.Email)
                .ToList();
            
            ViewBag.Customers = distinctCustomers;

            return View(messages);
        }
    
        [HttpGet]
        public async Task<IActionResult> GetMessagesByUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var messages = await _mongoContext.Messages
                                        .Find(m => m.User.Email == email)
                                        .SortByDescending(m => m.Timestamp)
                                        .ToListAsync();

            var messageData = messages.Select(m => new
            {
                sender = m.User.Email,
                text = m.Message
            }).ToList();

            return Json(messageData);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var nonAdminUsers = new List<Domain.Models.Auth.User>();
            foreach(var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                if(!role.Contains(StaticUserRole.ADMIN))
                {
                    nonAdminUsers.Add(user);
                }
            }
            return View(nonAdminUsers);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new JsonResult("No user found");
            }

            var currentRole = await _userManager.GetRolesAsync(user);
            var model = new EditUserRoleVM()
            {
                UserId = user.Id,
                CurrentRole = currentRole.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove user roles");
                return View(model);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add user role");
                return View(model);
            }

            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpGet]
        public async Task<IActionResult> UserDeatils(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return new JsonResult("The user with ID is not existed"); }

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new ProfileVM()
            {
                Id = userId,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                ImageUrl = null,
                Role = roles.FirstOrDefault(),
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Set lockout end date to a specific time in the future
            await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(7));

            // Optionally, update the IsLockedOut property
            user.LockoutEnabled = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(UserDeatils), new { userId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove lockout end date
            await _userManager.SetLockoutEndDateAsync(user, null);

            // Optionally, update the IsLockedOut property
            user.LockoutEnabled = false;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(UserDeatils), new { userId = userId });
        }


        [HttpGet]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var reports = await _order.GetOrderStatistics();
            return View(reports);
        }

        [HttpGet]
        public async Task<ActionResult> GetBrandSales()
        {
            var brandSales = await _context.OrderDetails
                .Include(p => p.Product)
                    .ThenInclude(b => b.Brands)
                .GroupBy(od => od.Product.Brands.BrandName)
                .Select(g => new BrandSalesVM(){
                    BrandName = g.Key,
                    TotalSales = g.Sum(od => od.Quantity * od.Product.ProductPrice)
                })
                .ToListAsync();
            return View(brandSales);
        }
    }
}
