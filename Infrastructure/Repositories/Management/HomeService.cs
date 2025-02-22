using Application.Interfaces.Management;
using Domain.Models.Management;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Size = Domain.Models.Management.Size;

namespace Infrastructure.Repositories.Management
{
    public class HomeService : IHomeService
    {
        private readonly ShelkobyPutDbContext _context;
        private Dictionary<int, Product> _productCache;

        public HomeService(ShelkobyPutDbContext context)
        {
            _context = context;
            _productCache = new Dictionary<int, Product>();
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<IEnumerable<Brands>> Brands()
        {
            return await _context.Brands.ToListAsync();
        }
        //public async Task<IEnumerable<Product>> GetAllproducts(int page, int pageSize)
        //{
        //    return await _context.Products.Include(p => p.Category).Include(s => s.Size)
        //        .Skip((page -1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<Product>> GetAllproducts(string sTerm = "", int categoryId = 0, int sizeId = 0, int page = 1, int pageSize = 12, int brandId = 0)
        {
            sTerm = sTerm.ToLower();

            var query = from product in _context.Products
                        join category in _context.Categories on product.CategoryId equals category.Id
                        join size in _context.Sizes on product.SizeId equals size.Id
                        join stock in _context.Stocks on product.Id equals stock.ProductId into product_stocks
                        from productWithStock in product_stocks.DefaultIfEmpty()
                        where (string.IsNullOrWhiteSpace(sTerm) || product.ProductName.ToLower().Contains(sTerm))
                              && (categoryId == 0 || product.CategoryId == categoryId)
                              && (sizeId == 0 || product.SizeId == sizeId)
                              && (brandId == 0 || product.BrandId == brandId)
                        select new Product
                        {
                            Id = product.Id,
                            Pictures = product.Pictures,
                            ProductName = product.ProductName,
                            CategoryId = product.CategoryId,
                            ProductPrice = product.ProductPrice,
                            Category = product.Category,
                            Size = product.Size,
                            SizeId = product.SizeId,
                            BrandId = product.BrandId,
                            Brands = product.Brands,
                            Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                        };

            var allProducts = query.ToList();
            var productsList = new List<Product>();
            foreach(var product in allProducts)
            {
                if(!_productCache.ContainsKey(product.Id))
                {
                    Console.WriteLine($"Cache hit for product ID: {product.Id}");
                    _productCache[product.Id] = product;
                }
                else
                {
                    Console.WriteLine($"Cache miss for product ID: {product.Id}");
                    productsList.Add(product);
                    _productCache[product.Id] = product;
                }
            }

            Console.WriteLine("products list: ");
            foreach(var i in productsList)
            {
                Console.WriteLine($"ID: {i.Id}, Name: {i.ProductName}");
            }

            var pagProducts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return pagProducts;
        }
        public async Task<int> GetTotalProductCount(string sterm, int categoryId)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(sterm))
            {
                query = query.Where(p => p.ProductName.Contains(sterm));
            }

            if (categoryId != 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            return await query.CountAsync();
        }
        public async Task<IEnumerable<Product>> GetNewProducts(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Product> products = await (from product in _context.Products
                                                   join category in _context.Categories
                                                   on product.CategoryId equals category.Id
                                                   join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId
                                                   into product_stocks
                                                   from productWithStock in product_stocks.DefaultIfEmpty()
                                                   where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm)))
                                                   && (categoryId == 0 || product.CategoryId == categoryId)
                                                   && product.CreatedDate >= DateTime.Now.AddDays(-30)
                                                   && productWithStock.Quantity > 0
                                                   select new Product
                                                   {
                                                       Id = product.Id,
                                                       Pictures = product.Pictures,
                                                       ProductName = product.ProductName,
                                                       CategoryId = product.CategoryId,
                                                       ProductPrice = product.ProductPrice,
                                                       Category = product.Category,
                                                       IsFeatured = product.IsFeatured,
                                                       CreatedDate = product.CreatedDate,
                                                       DiscountProductprice = product.DiscountProductprice,
                                                       Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                                                   }
                                    ).Take(5).ToListAsync();

            return products;
        }
        public async Task<IEnumerable<Product>> GetFeaturedProducts(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Product> products = await (from product in _context.Products
                                                   join category in _context.Categories
                                                   on product.CategoryId equals category.Id
                                                   join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId
                                                   into product_stocks
                                                   from productWithStock in product_stocks.DefaultIfEmpty()
                                                   where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm)))
                                                   && (categoryId == 0 || product.CategoryId == categoryId) && (product.IsFeatured == true)
                                                   select new Product
                                                   {
                                                       Id = product.Id,
                                                       Pictures = product.Pictures,
                                                       ProductName = product.ProductName,
                                                       CategoryId = product.CategoryId,
                                                       ProductPrice = product.ProductPrice,
                                                       Category = product.Category,
                                                       IsFeatured = product.IsFeatured,
                                                       DiscountProductprice = product.DiscountProductprice,
                                                       Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                                                   }
                                    ).Take(5).ToListAsync();

            return products;
        }
        //public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", int categoryId = 0)
        //{
        //    sTerm = sTerm.ToLower();
        //    IEnumerable<Product> products = await (from product in _context.Products
        //                                           join category in _context.Categories
        //                                           on product.CategoryId equals category.Id
        //                                           join stock in _context.Stocks
        //                                           on product.Id equals stock.ProductId
        //                                           into product_stocks
        //                                           from productWithStock in product_stocks.DefaultIfEmpty()
        //                                           where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm)))
        //                                           && (categoryId == 0 || product.CategoryId == categoryId)
        //                                           select new Product
        //                                           {
        //                                               Id = product.Id,
        //                                               Pictures = product.Pictures,
        //                                               ProductName = product.ProductName,
        //                                               CategoryId = product.CategoryId,
        //                                               ProductPrice = product.ProductPrice,
        //                                               Category = product.Category,
        //                                               Quantity = productWithStock == null ? 0 : productWithStock.Quantity
        //                                           }
        //                            ).Take(5).ToListAsync();

        //    return products;
        //}
        public async Task<IEnumerable<Brands>> GetBrands()
        {
            return await _context.Brands.ToListAsync();
        }
        public async Task<IEnumerable<Size>> GetSizes()
        {
            return await _context.Sizes.Include(p => p.Products).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", int categoryId = 0, int sizeId =0, int page = 1, int pageSize = 10, int brandId = 0)
        {
            sTerm = sTerm.ToLower();

            var query = from product in _context.Products
                        join category in _context.Categories on product.CategoryId equals category.Id
                        join size in _context.Sizes on product.SizeId equals size.Id
                        join stock in _context.Stocks on product.Id equals stock.ProductId into product_stocks
                        from productWithStock in product_stocks.DefaultIfEmpty()
                        where (string.IsNullOrWhiteSpace(sTerm) || product.ProductName.ToLower().Contains(sTerm))
                              && (categoryId == 0 || product.CategoryId == categoryId)
                              && (sizeId == 0 || product.SizeId == sizeId)
                              && (brandId == 0 || product.BrandId == brandId)
                        select new Product
                        {
                            Id = product.Id,
                            Pictures = product.Pictures,
                            ProductName = product.ProductName,
                            CategoryId = product.CategoryId,
                            ProductPrice = product.ProductPrice,
                            Category = product.Category,
                            Size = product.Size,
                            SizeId = product.SizeId,
                            BrandId = product.BrandId,
                            Brands = product.Brands,
                            Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                        };

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsSoldThisWeek(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            DateTime oneWeekAgo = DateTime.Now.AddDays(-7);

            // Truy vấn các sản phẩm từ bảng Product thông qua bảng Order và OrderDetails
            IEnumerable<Product> products = await (from product in _context.Products
                                                   join orderDetails in _context.OrderDetails
                                                   on product.Id equals orderDetails.ProductId
                                                   join order in _context.Orders
                                                   on orderDetails.OrderId equals order.Id
                                                   join category in _context.Categories
                                                   on product.CategoryId equals category.Id
                                                   join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId
                                                   into product_stocks
                                                   from productWithStock in product_stocks.DefaultIfEmpty()
                                                   where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm)))
                                                   && (categoryId == 0 || product.CategoryId == categoryId)
                                                   && order.CreatedDate >= oneWeekAgo // Kiểm tra xem đơn hàng được tạo trong vòng 7 ngày qua
                                                   //&& productWithStock.Quantity > 0 // Kiểm tra số lượng trong kho lớn hơn 0
                                                   orderby order.CreatedDate descending // Sắp xếp theo ngày tạo đơn hàng giảm dần
                                                   select new Product
                                                   {
                                                       Id = product.Id,
                                                       Pictures = product.Pictures,
                                                       ProductName = product.ProductName,
                                                       CategoryId = product.CategoryId,
                                                       ProductPrice = product.ProductPrice,
                                                       Category = product.Category,
                                                       Quantity = productWithStock.Quantity,
                                                   }
                                ).Take(3).ToListAsync(); // Lấy 3 sản phẩm gần nhất

            return products;
        }
        public async Task<IEnumerable<Product>> GetSaleProducts(string sTerm = "")
        {
            sTerm = sTerm.ToLower();

            IEnumerable<Product> products = await (from product in _context.Products
                                                   join category in _context.Categories
                                                   on product.CategoryId equals category.Id
                                                   join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId
                                                   into product_stocks
                                                   from productWithStock in product_stocks.DefaultIfEmpty()
                                                   where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm)))
                                                   && (product.DiscountProductprice != null && product.DiscountProductprice > 0)
                                                   select new Product
                                                   {
                                                       Id = product.Id,
                                                       Pictures = product.Pictures,
                                                       ProductName = product.ProductName,
                                                       CategoryId = product.CategoryId,
                                                       ProductPrice = product.ProductPrice,
                                                       DiscountProductprice = product.DiscountProductprice,
                                                       Category = product.Category,
                                                       Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                                                   }
                                ).Take(3).ToListAsync();

            return products;
        }
        public async Task<IEnumerable<Product>> GetBrandProducts(string sTerm = "")
        {
            sTerm = sTerm.ToLower();

            IEnumerable<Product> products = await (from product in _context.Products
                                                   join category in _context.Categories
                                                   on product.CategoryId equals category.Id
                                                   join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId
                                                   into product_stocks
                                                   from productWithStock in product_stocks.DefaultIfEmpty()
                                                   where (string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().Contains(sTerm)))
                                                   orderby productWithStock.Quantity ascending, product.BrandId
                                                   select new Product
                                                   {
                                                       Id = product.Id,
                                                       Pictures = product.Pictures,
                                                       ProductName = product.ProductName,
                                                       CategoryId = product.CategoryId,
                                                       ProductPrice = product.ProductPrice,
                                                       DiscountProductprice = product.DiscountProductprice,
                                                       Category = product.Category,
                                                       BrandId = product.BrandId,
                                                       Brands = product.Brands,
                                                       Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                                                   }
                                ).Take(3).ToListAsync();

            return products;
        }
    }
}
