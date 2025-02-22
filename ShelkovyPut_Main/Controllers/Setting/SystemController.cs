using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Setting
{
    public class SystemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetDirectoryContents")]
        public IActionResult GetDirectoryContents([FromQuery] string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return BadRequest("Path is required");
            }

            try
            {
                var result = new
                {
                    Directories = Directory.GetDirectories(path)
                        .Select(dir => new { Name = Path.GetFileName(dir), Path = dir }).ToList(),
                    Files = Directory.GetFiles(path)
                        .Select(file => new { Name = Path.GetFileName(file), Path = file }).ToList()
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
