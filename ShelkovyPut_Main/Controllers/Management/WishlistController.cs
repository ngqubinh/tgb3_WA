using System.Security.Claims;
using Application.Interfaces.Management;
using Application.ViewModels;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlist;
        private readonly ShelkobyPutDbContext _context;
        private readonly IHomeService _home;

        public WishlistController(IWishlistService wishlist, ShelkobyPutDbContext context, IHomeService home)
        {
            _wishlist = wishlist;
            _context = context;
            _home = home;
        }

        public async Task<IActionResult> Wishlists(string sTerm = "", int categoryId = 0) 
        {
            IEnumerable<WishList> wishLists = await _wishlist.GetAllWishListsAsync();
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandsForSearch = await _home.Brands();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandsForSearch,
                wishLists = wishLists
            };
            return viewModel == null ? NotFound() : View(viewModel);
        }

        public async Task<IActionResult> WishlistEmpty(string sTerm = "", int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            IEnumerable<Brands> brandsForSearch = await _home.Brands();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                BrandForSearch = brandsForSearch,
            };

            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var existingProduct = await _context.WishLists.FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);
            if (existingProduct != null)
            {
                TempData["Notification"] = "Sản phẩm này đã có trong wishlist của bạn!";
                var refererURL = Request.Headers["Referer"].ToString();
                return !string.IsNullOrEmpty(refererURL) ? Redirect(refererURL) : RedirectToAction(nameof(Wishlists));
            }

            var wishlistItem = new WishList
            {
                ProductId = productId,
                UserId = userId,
                CreatedDate = DateTime.Now
            };

            _context.WishLists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            var currentURL = Request.Headers["Referer"].ToString();
            return !string.IsNullOrEmpty(currentURL) ? Redirect(currentURL) : RedirectToAction(nameof(Wishlists));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) 
            {
                return NotFound("Khong tim thay san pham ton tai trong wishlist!");
            }

            var wishlistDetail = await _context.WishLists
                .Include(w => w.Product)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if(wishlistDetail == null)
            {
                return NotFound();
            }

            return View(wishlistDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        { 
            var wishlistItem = await _context.WishLists.FindAsync(id); 
            if (wishlistItem != null) { _context.WishLists.Remove(wishlistItem); 
            await _context.SaveChangesAsync(); } return RedirectToAction(nameof(Wishlists));
        }
    }
}
