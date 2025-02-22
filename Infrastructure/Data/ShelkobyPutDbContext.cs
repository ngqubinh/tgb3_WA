using Domain.Models.Auth;
using Domain.Models.Management;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ShelkobyPutDbContext : IdentityDbContext<User>
    {
        public ShelkobyPutDbContext(DbContextOptions<ShelkobyPutDbContext> options) : base(options) { }

        // Set the tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }

        // Rename the identity tables
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Config anything we want
            builder.Entity<User>(e =>
            {
                e.ToTable("Users");
            });
            builder.Entity<IdentityUserClaim<string>>(e =>
            {
                e.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(e =>
            {
                e.ToTable("UserLogins");
            });
            builder.Entity<IdentityUserToken<string>>(e =>
            {
                e.ToTable("UserTokens");
            });
            builder.Entity<IdentityRole>(e =>
            {
                e.ToTable("Roles");
            });
            builder.Entity<IdentityRoleClaim<string>>(e =>
            {
                e.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserRole<string>>(e =>
            {
                e.ToTable("UserRoles");
            });
        }
    }
}
