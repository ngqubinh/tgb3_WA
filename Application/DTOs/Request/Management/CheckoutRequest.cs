using Domain.Models.Management;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Management
{
    public class CheckoutRequest
    {
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }
        [Required]
        public string? MobileNumber { get; set; }
        //[Required]
        //[MaxLength(200)]
        //public string? Address { get; set; }
        [Required]
        public string? PaymentMethod { get; set; }
        public Address Address { get; set; }
    }
}
