using Application.Interfaces.Management;
using Application.ViewModels.Order;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShelkovyPut_Main.Controllers.Staff
{
    //[Authorize(Roles = StaticUserRole.STAFF)]
    public class ShipperController : Controller
    {
        private readonly IOrderService _order;
        private readonly ShelkobyPutDbContext _context;
        private readonly UserManager<Domain.Models.Auth.User> _userManager;

        public ShipperController(IOrderService order, ShelkobyPutDbContext context, UserManager<Domain.Models.Auth.User> userManager)
        {
            _order = order;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Staff/UnpaidOrders")]
        public async Task<IActionResult> UnpaidOrders()
        {
            var unpaidOrders = await _order.GetUnPaidOrders();
            return View(unpaidOrders);
        }        

        [HttpGet]
        public async Task<IActionResult> UnshippedOrders()
        {
            var unshippedOrders = await _order.GetUnshippedOrdersAsync();

            //var shippers = await _userManager.GetUsersInRoleAsync("STAFF");
            //ViewBag.Shippers = shippers;

            return View(unshippedOrders);
        }

        //[HttpPost]
        //public async Task<IActionResult> AssignShipperToOrders(List<UnshippedOrderVM> selectedOrders)
        //{
        //    if (selectedOrders == null || !selectedOrders.Any())
        //    {
        //        Console.WriteLine("No orders selected.");
        //        return RedirectToAction(nameof(UnshippedOrders));
        //    }

        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (currentUser == null)
        //    {
        //        Console.WriteLine("Current user not found.");
        //        return NotFound("No staff");
        //    }

        //    var currentUserId = currentUser.Id;

        //    Console.WriteLine("Orders selected for shipping:");
        //    foreach (var orderVM in selectedOrders)
        //    {
        //        if (orderVM.IsSelectedForShipping)
        //        {
        //            Console.WriteLine($"Order ID: {orderVM.OrderId}, Shipper ID: {orderVM.ShipperId}");
        //            var order = await _context.Orders.FindAsync(orderVM.OrderId);
        //            if (order != null)
        //            {
        //                order.IsShipped = true;

        //                var shipment = new Shipper
        //                {
        //                    ShippedDate = DateTime.Now,
        //                    UserId = currentUserId,
        //                    OrderId = order.Id
        //                };
        //                _context.Shippers.Add(shipment);
        //                await _context.SaveChangesAsync();
        //                order.ShipperId = shipment.Id;
        //                _context.Update(order);
        //                Console.WriteLine($"Assigned Shipper ID: {orderVM.ShipperId} to Order ID: {orderVM.OrderId}");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Order not found: {orderVM.OrderId}");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Order not selected for shipping: {orderVM.OrderId}");
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //    Console.WriteLine("Changes saved to the database.");
        //    return RedirectToAction(nameof(Success));
        //}

        [HttpPost]
        public async Task<IActionResult> AssignShipperToOrders(List<UnshippedOrderVM> selectedOrders)
        {
            if (selectedOrders == null || !selectedOrders.Any())
            {
                Console.WriteLine("No orders selected.");
                return RedirectToAction(nameof(UnshippedOrders));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("Current user not found.");
                return NotFound("No staff");
            }

            var currentUserId = currentUser.Id;

            Console.WriteLine("Orders selected for shipping:");
            foreach (var orderVM in selectedOrders)
            {
                if (orderVM.IsSelectedForShipping)
                {
                    Console.WriteLine($"Order ID: {orderVM.OrderId}, Shipper ID: {orderVM.ShipperId}");
                    var order = await _context.Orders.FindAsync(orderVM.OrderId);
                    if (order != null)
                    {
                        // Set order as shipped
                        order.IsShipped = true;

                        // Create a new Shipper entry
                        var shipment = new Shipper
                        {
                            ShippedDate = DateTime.Now,
                            UserId = currentUserId,
                            OrderId = order.Id
                        };

                        // Add the new shipper entry
                        _context.Shippers.Add(shipment);

                        // Save changes to generate the ID for the shipper
                        await _context.SaveChangesAsync();

                        // Update the order with the newly created shipper ID
                        order.ShipperId = shipment.Id;

                        // Update the order in the context
                        _context.Update(order);

                        Console.WriteLine($"Created new Shipper ID: {shipment.Id} for Order ID: {orderVM.OrderId}");
                    }
                    else
                    {
                        Console.WriteLine($"Order not found: {orderVM.OrderId}");
                    }
                }
                else
                {
                    Console.WriteLine($"Order not selected for shipping: {orderVM.OrderId}");
                }
            }

            // Save all changes at once to ensure data consistency
            await _context.SaveChangesAsync();
            Console.WriteLine("Changes saved to the database.");
            return RedirectToAction(nameof(Success));
        }




        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewTasks()
        {
            var userId = _userManager.GetUserId(User);
            var tasks = await _order.GetShipperTaskAsync(userId);
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsShipped(int orderId)
        {
            await _order.MarkOrderAsShippedAsync(orderId);
            return RedirectToAction(nameof(ViewTasks));
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _order.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return RedirectToAction(nameof(ViewTasks));
        }
    }
}
