using System.Security.Claims;
using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Domain.Constants;
using Domain.Models.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Auth
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _config;
		private readonly ShelkobyPutDbContext _context;
		private readonly ITokenService _token;
		private readonly IHttpContextAccessor _http;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, ShelkobyPutDbContext context, ITokenService token, IHttpContextAccessor http)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
            _token = token;
            _http = http;
        }

        public async Task<GeneralResponse> CreateAsync(SignupRequest model)
        {
            try
            {
                var user = await FindUserByEmailAsync(model.Email);
                if (user != null)
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = $"{model.Email} is already existed"
                    };
                }

                if (string.IsNullOrEmpty(model.Password) || !model.Password.Any(char.IsDigit) ||
                    !model.Password.Any(char.IsLetter) || !model.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = "Password must include at least one letter, one number, and one special character."
                    };
                }

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = "The ConfirmPassword field is required."
                    };
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = "'ConfirmPassword' and 'Password' do not match."
                    };
                }

                var newUser = new User()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = model.Password
                };

                var result = await _userManager.CreateAsync(newUser, model.Password!);
                string err = CheckResponse(result);
                if (!string.IsNullOrEmpty(err))
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = err
                    };
                }

                var assignRole = await _userManager.AddToRoleAsync(newUser, StaticUserRole.USER);
                if (!assignRole.Succeeded)
                {
                    return new GeneralResponse()
                    {
                        Success = false,
                        Message = "Can't assign role"
                    };
                }

                return new GeneralResponse()
                {
                    Success = true,
                    Message = "Register successsfully"
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }
        
        public async Task<UserResponse> LoginAsync(LoginRequest model)
        {
            try 
            {
                var user = await FindUserByEmail(model.Email);
                if(user == null)
                {
                    return new UserResponse()
                    {
                        Message = $"{user.Email} is not found"
                    };
                }

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if(!result.Succeeded)
                {
                    return new UserResponse()
                    {
                        Message = "The password is wrong"
                    };
                }    


                return await DisplayUserResponse(user);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResponse> RefreshPage()
		{
			var getUserFromCookie = _http.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(getUserFromCookie!);
			return await DisplayUserResponse(user!);
		}

		public async Task<UserResponse> RefreshToken()
		{
			try
			{
				var token = _http.HttpContext!.Request.Cookies["token"];
				var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

				var check = IsValidRefreshTokenAsync(userId, token!).GetAwaiter().GetResult();
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null)
				{
					return null!;
				}

				return await DisplayUserResponse(user);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        #region 
        private async Task<User> FindUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task<UserResponse> DisplayUserResponse(User user)
		{
			await SaveRefreshToken(user);
			var accessToken = await _token.CreateAccessToken(user);
			var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!.ToString();
			return new UserResponse()
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				AccessToken = accessToken,
				Role = role,
				Message = ""
			};
		}

		private async Task SaveRefreshToken(User user)
		{
			var refreshToken = _token.CreateRefreshToken(user);

			var existingRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.UserID == user.Id);
			if (existingRefreshToken != null)
			{
				existingRefreshToken.Token = refreshToken.Token;
				existingRefreshToken.DateCreatedUtc = refreshToken.DateCreatedUtc;
				existingRefreshToken.DateExpiresUtc = refreshToken.DateExpiresUtc;
			}
			else
			{
				user.RefreshTokens.Add(refreshToken);
			}

			await _context.SaveChangesAsync();

			var cookieOptions = new CookieOptions
			{
				Expires = refreshToken.DateExpiresUtc,
				IsEssential = true,
				HttpOnly = true,
			};

			_http.HttpContext!.Response.Cookies.Append("token", refreshToken.Token!, cookieOptions);
		}

		private async Task<bool> IsValidRefreshTokenAsync(string userId, string token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return false;

			var fetchedRefreshToken = await _context.RefreshTokens
				.FirstOrDefaultAsync(x => x.UserID == userId && x.Token == token);
			if (fetchedRefreshToken == null) return false;
			if (fetchedRefreshToken.IsExpired) return false;

			return true;
		}

        private static string CheckResponse(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var err = result.Errors.Select(i => i.Description);
                return string.Join(Environment.NewLine, err);
            }

            return null!;
        }

        private async Task<User> FindUserByEmailAsync(string email)
        {
            var userEmail = await _userManager.FindByEmailAsync(email);
            return userEmail;
        }
        #endregion
    }
}
