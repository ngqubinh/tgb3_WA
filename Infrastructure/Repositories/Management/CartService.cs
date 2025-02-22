using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Enum;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Repositories.Management
{
    public class CartService : ICartService
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<User> _userManager;
        private readonly IVnPayService _vnpay;

        public CartService(ShelkobyPutDbContext context, IHttpContextAccessor http, UserManager<User> userManager, IVnPayService vnPay)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
            _vnpay = vnPay;
        }

        public async Task<int> AddItem(int productId, int qty)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var product = await _context.Products.FindAsync(productId);
                        if (product == null)
                        {
                            throw new Exception($"product with {productId} does not exist");
                        }

                        string userId = GetUserId();
                        if (string.IsNullOrEmpty(userId))
                        {
                            throw new UnauthorizedAccessException("User is not logged-in");
                        }

                        var cart = await GetCart(userId!);
                        if (cart == null)
                        {
                            cart = new ShoppingCart
                            {
                                UserId = userId,
                            };
                            _context.ShoppingCarts.Add(cart);
                            await _context.SaveChangesAsync();
                        }



                        var cartItem = _context.CartDetails.FirstOrDefault(c => c.ShoppingCartId == cart.Id && c.ProductId == productId);
                        if (cartItem != null)
                        {
                            cartItem.Quantity += qty;
                        }
                        else
                        {
                            cartItem = new CartDetails
                            {
                                ProductId = productId,
                                ShoppingCartId = cart.Id,
                                Quantity = qty,
                                UnitPrice = product.ProductPrice
                            };
                            _context.CartDetails.Add(cartItem);
                        }

                        await _context.SaveChangesAsync();
                        transaction.Commit();

                        return await GetCartItemCount(userId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            });
        }
        public async Task AddMultipleItemsAsync(string userId, List<CartDetails> items)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var cart = await _context.ShoppingCarts
                            .Include(c => c.CartDetails)
                                .ThenInclude(p => p.Product)
                            .FirstOrDefaultAsync(c => c.UserId == userId);

                        if (cart == null)
                        {
                            cart = new ShoppingCart { UserId = userId };
                            _context.ShoppingCarts.Add(cart);
                            await _context.SaveChangesAsync();
                        }

                        foreach (var item in items)
                        {
                            var product = await _context.Products.FindAsync(item.ProductId);
                            if (product == null)
                            {
                                throw new Exception($"product with {item.ProductId} does not exist");
                            }

                            var cartItem = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == item.ProductId);
                            if (cartItem != null)
                            {
                                cartItem.Quantity += item.Quantity;
                                cartItem.UnitPrice = product.ProductPrice;
                            }
                            else
                            {
                                cartItem = new CartDetails
                                {
                                    ProductId = item.ProductId,
                                    ShoppingCartId = cart.Id,
                                    Quantity = item.Quantity = 1,
                                    UnitPrice = product.ProductPrice
                                };
                                _context.CartDetails.Add(cartItem);
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("An error occurred while adding multiple items to the cart: " + ex.Message);
                    }
                }
            });
        }


        public async Task<bool> DoCheckout(CheckoutRequest model)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var userId = GetUserId();
                        if (string.IsNullOrEmpty(userId))
                        {
                            throw new UnauthorizedAccessException("User is not logged-in");
                        }

                        var cart = await GetCart(userId);
                        if (cart is null)
                        {
                            throw new InvalidOperationException("Invalid cart");
                        }

                        var cartDetail = _context.CartDetails.Where(c => c.ShoppingCartId == cart.Id).ToList();
                        if (cartDetail.Count == 0)
                        {
                            throw new InvalidOperationException("Cart is empty");
                        }

                        var pendingRecord = _context.OrderStatus.FirstOrDefault(s => s.StatusName == StaticOrderStatus.ConfirmedOrder);
                        if (pendingRecord == null)
                        {
                            throw new InvalidOperationException("Order status does not have Pending status");
                        }

                        var order = new Order
                        {
                            UserId = userId,
                            CreatedDate = DateTime.Now,
                            Name = model.Name,
                            Email = model.Email,
                            MobileNumber = model.MobileNumber,
                            PaymentMethod = model.PaymentMethod,
                            Addresses = model.Address,
                            IsPaid = false,
                            OrderStatusId = pendingRecord.Id
                        };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        foreach (var item in cartDetail)
                        {
                            var product = await _context.Products.FindAsync(item.ProductId);
                            if (product == null)
                            {
                                throw new InvalidOperationException("Product not found");
                            }

                            // Cập nhật câu điều kiện để đảm bảo UnitPrice luôn có giá trị
                            var unitPrice = product.DiscountProductprice.HasValue && product.DiscountProductprice.Value > 0
                                            ? product.DiscountProductprice.Value
                                            : product.ProductPrice;

                            var orderDetail = new OrderDetails
                            {
                                ProductId = item.ProductId,
                                OrderId = order.Id,
                                Quantity = item.Quantity,
                                UnitPrice = unitPrice // Giá sản phẩm (có thể đã giảm)
                            };
                            _context.OrderDetails.Add(orderDetail);

                            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId);
                            if (stock == null)
                            {
                                throw new InvalidOperationException("Stock is null");
                            }
                            if (item.Quantity > stock.Quantity)
                            {
                                throw new InvalidOperationException($"Only {stock.Quantity} items(s) are available in the stock");
                            }
                            stock.Quantity -= item.Quantity;
                        }

                        _context.CartDetails.RemoveRange(cartDetail);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            });
        }



        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            }
            var data = await (from cart in _context.ShoppingCarts
                              join cartDetail in _context.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                              ).ToListAsync();
            return data.Count;
        }
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new Exception("User not found");
            }
            var shoppingCart = await _context.ShoppingCarts.Include(a => a.CartDetails)
                .ThenInclude(a => a.Product).ThenInclude(a => a.Stock)
                .Include(a => a.CartDetails)
                    .ThenInclude(a => a.Product)
                    .ThenInclude(a => a.Category)
                .Include(a => a.CartDetails)
                    .ThenInclude(a => a.Product)
                    .ThenInclude(p => p.Size)
                .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;
        }

        public async Task<bool> RemoveFromCart(int productId)
        {
            var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                Console.WriteLine("User is not logged in.");
                return false;
            }

            var cart = await _context.ShoppingCarts
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                Console.WriteLine("Cart not found for user.");
                return false;
            }

            var cartItem = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
            if (cartItem == null)
            {
                Console.WriteLine("Cart item not found. ProductId: " + productId);
                throw new Exception("Cart item not found.");
            }

            _context.CartDetails.Remove(cartItem);
            await _context.SaveChangesAsync();

            Console.WriteLine("Item removed successfully. ProductId: " + productId);
            return true;
        }


        public async Task<int> RemoveItem(int productId)
        {
            string userId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                if (userId == null)
                {
                    throw new Exception("User not found");
                }
                var cart = await GetCart(userId!);
                if (cart == null)
                {
                    throw new Exception("cart not found");
                }
                await _context.SaveChangesAsync();

                var cartItem = _context.CartDetails.FirstOrDefault(c => c.ShoppingCartId == cart.Id && c.ProductId == productId);
                if (cartItem == null)
                {
                    throw new Exception("cart not found"); ;
                }
                else if (cartItem.Quantity == 1)
                {
                    _context.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;
                }
                await _context.SaveChangesAsync();
                //transaction.Commit();
                //return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        #region
        private async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        private string GetUserId()
        {
            var principal = _http.HttpContext!.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
        #endregion
    }
}
