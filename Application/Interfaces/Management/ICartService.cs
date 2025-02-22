using Application.DTOs.Request.Management;
using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface ICartService
    {
        Task<int> AddItem(int productId, int qty);
        Task<int> RemoveItem(int productId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<bool> DoCheckout(CheckoutRequest model);
        Task<bool> RemoveFromCart(int productId);
        Task AddMultipleItemsAsync(string userId, List<CartDetails> items);
    }
}
