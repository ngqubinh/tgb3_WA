using Application.Interfaces.Management;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Management
{
    public class WishListService : IWishlistService
    {
        private readonly ShelkobyPutDbContext _context;

        public WishListService(ShelkobyPutDbContext context)
        {
            _context = context;
        }

        public Task<List<WishList>> GetAllWishListsAsync()
        {
            var wishListItems = _context.WishLists
                .Include(w => w.Product)
                .Include(w => w.User)
                    .ToListAsync();
            
            return wishListItems;
        }
    }
}