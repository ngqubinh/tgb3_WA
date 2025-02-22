namespace Application.ViewModels.User
{
    public class UserDashboardVM
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public IEnumerable<MyOrderVM>? Orders { get; set; }
    }
}
