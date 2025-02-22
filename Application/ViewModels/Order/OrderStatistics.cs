namespace Application.ViewModels.Order
{
    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public double TotalSales { get; set; }
        public int TotalCustomers { get; set; }
        public List<TopProduct> TopProducts { get; set; }
        public List<BrandSalesVM> BrandSalesVMs { get; set; }
    }
}
