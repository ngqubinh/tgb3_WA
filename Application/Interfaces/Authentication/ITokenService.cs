using Domain.Models.Auth;

namespace Application.Interfaces.Authentication
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(User user);
		RefreshTokens CreateRefreshToken(User user);
    }
}