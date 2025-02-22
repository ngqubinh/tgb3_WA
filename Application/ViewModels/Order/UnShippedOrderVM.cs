namespace Application.ViewModels.Order
{
    public class UnshippedOrderVM 
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public double TotalAmount { get; set; }
        public bool IsSelectedForShipping { get; set; }
        public string ShipperId { get; set; }
        public string? CurrentShipperName { get; set; }
        public List<Domain.Models.Auth.User> AvailableShippers { get; set; }
    }

}