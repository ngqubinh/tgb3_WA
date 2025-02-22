namespace Application.DTOs.Response.Management
{
    public class StockResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
    }
}
