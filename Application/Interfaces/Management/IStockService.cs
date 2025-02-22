using Application.DTOs.Request.Management;
using Application.DTOs.Response.Management;
using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface IStockService
    {
        Task<IEnumerable<StockResponse>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockProductId(int productId);
        Task ManageStock(StockRequest stockToManage);
    }
}
