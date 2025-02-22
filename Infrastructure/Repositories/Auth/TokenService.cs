using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Authentication;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _congfig;
        private readonly SymmetricSecurityKey _jwtKey;
		private readonly UserManager<User> _userManager;

		public TokenService(IConfiguration congfig, UserManager<User> userManager)
		{
			_congfig = congfig;
			_jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_congfig["JWT:Key"]!));
			_userManager = userManager;
		}

		public async Task<string> CreateAccessToken(User user)
		{
			var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!.ToString();
			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email!),
				new Claim(ClaimTypes.Role, role!)
			};

			var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);
			var tokenDesciptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(userClaims),
				Expires = DateTime.UtcNow.AddDays(int.Parse(_congfig["JWT:ExpiresInMinutes"]!)),
				SigningCredentials = credentials,
				Issuer = _congfig["JWT:Issuer"],
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwt = tokenHandler.CreateToken(tokenDesciptor);

			return tokenHandler.WriteToken(jwt);
		}

		public RefreshTokens CreateRefreshToken(User user)
		{
			var token = new byte[32];
			using var randomNumber = RandomNumberGenerator.Create();
			randomNumber.GetBytes(token);

			var refreshToken = new RefreshTokens()
			{
				Token = Convert.ToBase64String(token),
				User = user,
				DateExpiresUtc = DateTime.UtcNow.AddDays(int.Parse(_congfig["JWT:RefreshTokenExpiresInDays"]!))
			};

			return refreshToken;
		}
    }
}