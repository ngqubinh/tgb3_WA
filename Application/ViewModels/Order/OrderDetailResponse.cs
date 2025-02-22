using Domain.Models.Management;

namespace Application.ViewModels.Order
{
    public class OrderDetailResponse
    {
        public string? DivId { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
