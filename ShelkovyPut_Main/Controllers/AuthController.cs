using Application.Interfaces;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        // [HttpPost]
        // public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         await _auth.ForgotPassword(model);
        //     }


        //     return RedirectToAction(nameof(ForgotPasswordConfirmation));
        // }

        // public IActionResult ForgotPasswordConfirmation()
        // {
        //     return View();
        // }
    }    
}
