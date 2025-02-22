using Application.DTOs.Request.Management;
using Application.DTOs.Response.Management;
using Application.Interfaces.Management;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Management
{
    public class StockService : IStockService
    {
        private readonly ShelkobyPutDbContext _context;

        public StockService(ShelkobyPutDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockProductId(int productId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
        }

        public async Task<IEnumerable<StockResponse>> GetStocks(string sTerm = "")
        {
            var stocks = await (from product in _context.Products
                                join stock in _context.Stocks
                                on product.Id equals stock.ProductId
                                into product_stock
                                from productStock in product_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || product.ProductName!.ToLower().Contains(sTerm.ToLower())
                                    
                                select new StockResponse
                                {
                                    ProductId = product.Id,
                                    ProductName = product.ProductName,
                                    Quantity = productStock == null ? 0 : productStock.Quantity
                                }
                                ).ToListAsync();
            return stocks;
        }

        public async Task ManageStock(StockRequest stockToManage)
        {
            var existingStock = await GetStockProductId(stockToManage.ProductId);
            if (existingStock is null)
            {
                var stock = new Stock { ProductId = stockToManage.ProductId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }
    }
}
