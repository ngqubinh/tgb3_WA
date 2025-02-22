using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{
    public class Brands
    {
        [Key]
        public int Id { get; set; }
        public string BrandName { get; set; }
    }
}
