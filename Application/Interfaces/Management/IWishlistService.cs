using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface IWishlistService
    {
        Task<List<WishList>> GetAllWishListsAsync();
    }
}