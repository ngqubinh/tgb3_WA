using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;
using Application.ViewModels.Auth;

namespace Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<UserResponse> LoginAsync(LoginRequest model);
        Task<UserResponse> RefreshPage();
		Task<UserResponse> RefreshToken();
        Task<GeneralResponse> CreateAsync(SignupRequest model);
    }
}