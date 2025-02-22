namespace Application.ViewModels.User
{
    public class UserOrderGroup
    {
        public ProfileVM? ProfileVM { get; set; }
        public IEnumerable<MyOrderVM> MyOrdersVM { get; set; }
    }   
}
