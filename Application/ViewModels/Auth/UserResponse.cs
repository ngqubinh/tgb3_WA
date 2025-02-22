namespace Application.ViewModels.Auth
{
    public class UserResponse
    {
        public string? Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? Role { get; set; }
		public string? AccessToken { get; set; }
		public string? Message { get; set; }
    }
}