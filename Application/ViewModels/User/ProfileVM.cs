using Application.ViewModels.Order;
using Domain.Models.Management;

namespace Application.ViewModels.User
{
    public class ProfileVM
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? BirhtDate { get; set; }
        public string? Sex { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Role { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }

        public IEnumerable<MyOrderVM> Orders { get; set; }
        public UserOrderDetailVM? OrderDetail { get; set; }
        public IEnumerable<Category>? CategoryForSearch { get; set; }
        public IEnumerable<Brands>? BrandForSearch { get; set; }
        public TrackOrderVM? TrackOrder { get; set; }
    }
}
