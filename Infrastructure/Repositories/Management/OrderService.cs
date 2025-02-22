using Application.Interfaces.Management;
using Application.ViewModels.Order;
using Application.ViewModels.User;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.Management
{
    public class OrderService : IOrderService
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly IHttpContextAccessor _http;
        private UserManager<User> _userManager;

        public OrderService(ShelkobyPutDbContext context, IHttpContextAccessor http, UserManager<User> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }
        public async Task ChangeOrderStatus(UpdateOrderStatusVM model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order withi id:{model.OrderId} does not found");
            }
            order.OrderStatusId = model.OrderStatusId;
            await _context.SaveChangesAsync();
        }
        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }
        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _context.OrderStatus.ToListAsync();
        }
        public async Task TogglePaymentStatus(int orderID)
        {
            var order = await _context.Orders.FindAsync(orderID);
            if (order == null)
            {
                throw new InvalidOperationException($"order withi id:{orderID} does not found");
            }
            order.IsPaid = !order.IsPaid;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Order>> AdminOrders(bool getAll = false)
        {
            var orders = _context.Orders.Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails).ThenInclude(o => o.Product).ThenInclude(o => o.Category).AsQueryable();

            if(!getAll)
            {
                var userId = GetUserId();
                if(string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged");
                }

                orders = orders.Where(x => x.UserId == userId);
                return await orders.ToListAsync();
            }

            return await orders.ToListAsync();
        }
        
        public async Task<ProfileVM> UserOrders(bool getAll = false)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) { throw new UnauthorizedAccessException("User is not logged"); }

            var ordersQuery = _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product).ThenInclude(p => p.Category)
                .Include(o => o.User)
                .AsQueryable();
            if (!getAll) { ordersQuery = ordersQuery.Where(o => o.UserId == userId); }

            var user = await _context.Users.FindAsync(userId);
            if(user == null) { throw new Exception("Cannot get user id"); }

            var categories = await _context.Categories.ToListAsync();
            var brandsForSearch = await _context.Brands.ToListAsync();
            var orders = await ordersQuery.ToListAsync();

            var profileVM = new ProfileVM()
            {
                Id = user?.Id,
                Email = user!.Email,
                UserName = user!.UserName,
                FullName = user?.FullName,
                PhoneNumber = user?.PhoneNumber,
                ImageUrl = user?.Image,
                BirhtDate = user?.BirthDate,
                Sex = user?.Sex,
                CreatedDate = user?.CreatedDate,
                IsEmailConfirmed = user?.EmailConfirmed ?? false,
                IsPhoneNumberConfirmed = user?.PhoneNumberConfirmed ?? false,
                Orders = orders.Select(order => new MyOrderVM
                {
                    OrderId = order.Id,
                    CreatedDate = order.CreatedDate,
                    OrderStatus = order.OrderStatus,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailVM
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName,
                        Pictures = od.Product?.Pictures,
                        Quantity = od.Quantity,
                        TotalPrice = od.Product?.ProductPrice * od.Quantity ?? 0,
                    }).ToList(),
                    TotalOrderPrice = order.OrderDetails.Sum(od => od.Product?.ProductPrice * od.Quantity ?? 0),
                    FullName = order.User.FullName,
                    Email = order.User.Email
                }).ToList(),
                CategoryForSearch = categories,
                BrandForSearch = brandsForSearch 
            };

            return profileVM;
        }

        public async Task<IEnumerable<MyOrderVM>> GetOrdersByFilter(string userId, string filter)
        {
            IQueryable<Order> ordersQuery = _context.Orders.Where(o => o.UserId == userId);

            switch(filter)
            {
                case "last5":
                    ordersQuery = ordersQuery.OrderByDescending(o => o.CreatedDate).Take(10);
                    break;
                case "last15days":
                    var fifteenDaysAgo = DateTime.Now.AddDays(-15);
                    ordersQuery = ordersQuery.Where(o => o.CreatedDate >=fifteenDaysAgo);
                    break;
                case "last30days":
                    var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                    ordersQuery = ordersQuery.Where(o => o.CreatedDate >= thirtyDaysAgo);
                    break;
                case "last6months":
                    var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                    ordersQuery = ordersQuery.Where(o => o.CreatedDate >= sixMonthsAgo);
                    break;
                case "all":
                default:
                    break;
            }

            var orders = await ordersQuery.Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .Include(o => o.OrderStatus)
                                .ToListAsync();
            var orderVM = orders.Select(o => new MyOrderVM()
            {
                OrderId = o.Id,
                CreatedDate = o.CreatedDate,
                OrderStatus = o.OrderStatus,
                OrderDetails = o.OrderDetails.Select(i => new OrderDetailVM()
                {
                    ProductName = i.Product?.ProductName,
                    Quantity = i.Quantity,
                    TotalPrice = i.Product.ProductPrice * i.Quantity
                }).ToList(),                
            }).ToList();

            return orderVM;
        }
        public async Task<ProfileVM> GetOrderDetails(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.User)
                .Include(o => o.Addresses)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            var categories = await _context.Categories.ToListAsync();
            var brandsForSearch = await _context.Brands.ToListAsync();

            var profileVM = new ProfileVM
            {
                Id = order.User?.Id,
                Email = order.User.Email,
                UserName = order.User?.UserName,
                FullName = order.User?.FullName,
                PhoneNumber = order.User?.PhoneNumber,
                ImageUrl = order.User?.Image,
                CreatedDate = order.User?.CreatedDate,
                //Role = order.User?.Role,
                //IsLockedOut = order.User?.IsLockedOut ?? false,
                IsEmailConfirmed = order.User?.EmailConfirmed ?? false,
                IsPhoneNumberConfirmed = order.User?.PhoneNumberConfirmed ?? false,
                Orders = new List<MyOrderVM>(),
                OrderDetail = new UserOrderDetailVM
                {
                    OrderId = order.Id,
                    CreatedDate = order.CreatedDate,
                    OrderStatus = order.OrderStatus?.StatusName,
                    UserName = order.User?.UserName,
                    UserEmail = order.User?.Email,
                    PhoneNumber = order.User?.PhoneNumber,
                    Settlement = order.Addresses?.Settlement,
                    District = order.Addresses?.District,
                    City = order.Addresses?.City,
                    Province = order.Addresses?.Province,
                    PaymentMethods = order.PaymentMethod,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailVM
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName,
                        Quantity = od.Quantity,
                        TotalPrice = od.Product?.ProductPrice * od.Quantity ?? 0,
                        Pictures = od.Product?.Pictures
                    }).ToList()
                }, 
                CategoryForSearch = categories,
                BrandForSearch = brandsForSearch,
            };

            return profileVM;
        }
        public async Task<List<UnPaidOrderVM>> GetUnPaidOrders()
        {
            var unpaidOrders = await _context.Orders
                .Where(o => !o.IsPaid)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Select(o => new UnPaidOrderVM()
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedDate,
                    CustomerName = o.User.Email,
                    TotalAmout = o.OrderDetails.Sum(od => od.Quantity * od.Product.ProductPrice),
                    IsSelectedForShipping = false
                }).ToListAsync();

            return unpaidOrders;
        }
        public async Task<List<UnshippedOrderVM>> GetUnshippedOrdersAsync() 
        {
            var unshippedOrders = await _context.Orders
                .Where(o => !o.IsShipped)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Include(o => o.Shipper).ThenInclude(s => s.User)
                .Select(o => new UnshippedOrderVM {
                    OrderId = o.Id,
                    OrderDate = o.CreatedDate,
                    CustomerName = o.User.FullName,
                    TotalAmount = o.OrderDetails.Sum(od => od.Quantity * od.Product.ProductPrice),
                    IsSelectedForShipping = false,
                    CurrentShipperName = o.Shipper != null ? o.Shipper.User.Email : "Not Assigned",
                    AvailableShippers = new List<User>()
                }).ToListAsync();

            return unshippedOrders;
        }
        public async Task AssignShipperToOrdersAsync(List<UnshippedOrderVM> model)
        {
            try 
            {
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.ShipperId))
                    {
                        var order = await _context.Orders.FindAsync(item.OrderId);
                        if (order != null && !order.IsShipped)
                        {
                            // var shipper = await _context.Shippers.FirstOrDefaultAsync(s => s.UserId==item.ShipperId);
                            // if(shipper != null && shipper.OrderId == null)
                            // {
                            //     // var shipperId = await GetOrCreateShipperId(shipper.UserId, order.Id);
                            //     // order.ShipperId = shipperId;
                            //     shipper.OrderId = order.Id;
                            //     _context.Shippers.Update(shipper);

                            //     order.ShipperId = shipper.Id;
                            //     _context.Orders.Update(order);
                            // }
                            // else 
                            // {
                            //     Console.WriteLine($"Shipper {item.ShipperId} is currently busy.");
                            // }

                            // Gán UserId của Shipper cho Order
                            var shipperUserId = item.ShipperId;
                            order.ShipperId = await GetOrCreateShipperId(shipperUserId, order.Id);
                            _context.Orders.Update(order);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                var hubContext = _http.HttpContext.RequestServices.GetRequiredService<IHubContext<Notification>>();
                await hubContext.Clients.All.SendAsync("ReceiveMessage", $"An error occurred: {ex.Message}");
            }
        }   
        public async Task<List<UnshippedOrderVM>> GetShipperTaskAsync(string shipperId)
        {
            var tasks = await _context.Orders
                .Where(o => o.Shipper.UserId == shipperId && !o.IsShipped)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Select(o => new UnshippedOrderVM() {
                    OrderId = o.Id,
                    OrderDate = o.CreatedDate,
                    CustomerName = o.User.Email,
                    TotalAmount = o.OrderDetails.Sum(od => od.Quantity * od.Product.ProductPrice)
                }).ToListAsync();

            return tasks;
        }    
        public async Task MarkOrderAsShippedAsync(int orderId) {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null) {
                order.IsShipped = true;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
        private async Task<int> GetOrCreateShipperId(string userId, int orderId)
        {
            var shipper = await _context.Shippers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (shipper == null)
            {
                shipper = new Shipper
                {
                    UserId = userId,
                    OrderId = orderId,
                    ShippedDate = DateTime.Now
                };
                _context.Shippers.Add(shipper);
                await _context.SaveChangesAsync();
            }
            else
            {
                if(shipper.OrderId != orderId)
                {
                    var oldOrder = await _context.Orders.FindAsync(shipper.OrderId);
                    if(oldOrder != null && !oldOrder.IsShipped)
                    {
                        oldOrder.ShipperId = null;
                        _context.Orders.Update(oldOrder);
                    }

                    shipper.OrderId = orderId;
                    _context.Shippers.Update(shipper);
                    await _context.SaveChangesAsync();
                }
            }
            return shipper.Id;
        }
        public async Task<OrderStatistics> GetOrderStatistics()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Brands)
                .ToListAsync();

            var totalSales = orders.Sum(o => o.OrderDetails.Sum(od => od.Product.ProductPrice * od.Quantity));
            var totalOrders = orders.Count;
            var totalCustomers = orders.Select(o => o.UserId).Distinct().Count();

            var topProducts = orders.SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.Product.ProductName)
                .Select(g => new TopProduct
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    TotalSales = g.Sum(od => od.Product.ProductPrice * od.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToList();

            var brandSales = orders.SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.Product.Brands.BrandName)
                .Select(g => new BrandSalesVM
                {
                    BrandName = g.Key,
                    TotalSales = g.Sum(od => od.Product.ProductPrice * od.Quantity)
                })
                .ToList();

            return new OrderStatistics
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                TotalCustomers = totalCustomers,
                TopProducts = topProducts,
                BrandSalesVMs = brandSales
            };
        }

        public async Task<Order> GetOrderByEmailAndId(string email, int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Email == email && o.Id == orderId);
        }

        #region
        private string GetUserId()
        {
            var principal = _http.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }        
        #endregion
    }
}
