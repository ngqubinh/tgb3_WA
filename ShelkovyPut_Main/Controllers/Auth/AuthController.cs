using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Auth
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _auth;

		public AuthController(IAuthService auth)
		{
			_auth = auth;
		}

		[HttpPost("Register")]
		public async Task<ActionResult<GeneralResponse>> Register(SignupRequest model)
		{
			var registerUser = await _auth.CreateAsync(model);
            if (!registerUser.Success)
            {
                return BadRequest(registerUser);
            }
            return Ok(registerUser);
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserResponse>> Login(LoginRequest model)
		{
			var loginUser = await _auth.LoginAsync(model);
			return Ok(loginUser);
		}

		[Authorize]
		[HttpGet("refresh-page")]
		public async Task<ActionResult<UserResponse>> RefreshPage()
		{
			var user = await _auth.RefreshPage();
			return Ok(user);
		}

		[Authorize]
		[HttpPost("refresh-token")]
		public async Task<ActionResult<UserResponse>> RefreshToken()
		{
			return await _auth.RefreshToken();
		}
	}
}