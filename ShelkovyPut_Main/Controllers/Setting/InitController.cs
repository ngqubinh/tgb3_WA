using Application.DTOs.Response;
using Application.Interfaces.Setting;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitController : ControllerBase
    {
        private readonly IInitService _init;

        public InitController(IInitService init)
        {
            _init = init;
        }

        [HttpPost("Seed-Roles")]
        public async Task<ActionResult<GeneralResponse>> SeedRoles()
        {
            var seedRoles = await _init.SeedRoles();
            return Ok(seedRoles);
        }

        [HttpPost("Create-Admin")]
        public async Task<ActionResult<GeneralResponse>> CreateAdmin()
        {
            var createAdmin = await _init.CreateInitAdmin();
            return Ok(createAdmin);
        }

        [HttpPost("SeedOrderStatus")]
        public async Task<ActionResult<GeneralResponse>> SeedOrderStatus()
        {
            var response = await _init.SeedOrderStatus();
            return Ok(response);
        }
    }
}
