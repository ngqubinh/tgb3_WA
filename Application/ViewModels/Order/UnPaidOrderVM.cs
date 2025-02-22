using Domain.Models.Management;

namespace Application.ViewModels.Order
{
    public class UnPaidOrderVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public double TotalAmout { get; set; }
        public bool IsSelectedForShipping { get; set; }
        public string? UserId { get; set; }
    }
}
