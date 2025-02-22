using Domain.Models.Enum;
using Domain.Models.Management;

namespace Application.ViewModels.User
{
    public class UserOrderDetailVM
    {
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Settlement {  get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PaymentMethods { get; set; }
        public IEnumerable<OrderDetailVM> OrderDetails { get; set; }        
    }
}
