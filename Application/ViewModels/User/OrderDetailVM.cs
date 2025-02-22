namespace Application.ViewModels.User
{
    public class OrderDetailVM
    {
        public int ProductId { get; set; } 
        public string? ProductName { get; set; }
        public string Pictures { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
