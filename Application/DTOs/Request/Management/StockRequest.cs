using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Management
{
    public class StockRequest
    {
        public int ProductId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }
    }
}
