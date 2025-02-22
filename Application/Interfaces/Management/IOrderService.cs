using Application.ViewModels.Order;
using Application.ViewModels.User;
using Domain.Models.Management;

namespace Application.Interfaces.Management
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> AdminOrders(bool getAll = false);
        Task<ProfileVM> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusVM model);
        Task TogglePaymentStatus(int orderID);
        Task<Order> GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();
        Task<IEnumerable<MyOrderVM>> GetOrdersByFilter(string userId, string filter);
        Task<ProfileVM> GetOrderDetails(int orderId);
        Task<List<UnPaidOrderVM>> GetUnPaidOrders();
        Task<List<UnshippedOrderVM>> GetUnshippedOrdersAsync();
        Task AssignShipperToOrdersAsync(List<UnshippedOrderVM> selectedOrders);
        Task<List<UnshippedOrderVM>> GetShipperTaskAsync(string shipperId);
        Task MarkOrderAsShippedAsync(int orderId);
        Task<OrderStatistics> GetOrderStatistics();
        Task<Order> GetOrderByEmailAndId(string email, int orderId);
    }
}
