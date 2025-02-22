using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Order
{
    public class UpdateOrderStatusVM
    {
        public int OrderId { get; set; }
        [Required]
        public int OrderStatusId { get; set; }
        public IEnumerable<SelectListItem>? OrderStatusList { get; set; }
    }
}
