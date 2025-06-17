using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var loginResult = await _loginService.LoginAsync(username, password);

            if (loginResult != null)
            {
                TempData["Message"] = loginResult.Message;

                if (loginResult.Success && !string.IsNullOrEmpty(loginResult.Token))
                {
                    HttpContext.Session.SetString("JWToken", loginResult.Token);
                    return RedirectToAction("GetAll", "Employee");
                }
                else
                {
                
                    TempData["IsSuccess"] = false;
                }
            }
            else
            {
                TempData["Message"] = "Không nhận được phản hồi từ server.";
                TempData["IsSuccess"] = false;
            }

            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var result = await _loginService.Register(username, password);

            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.Success;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            TempData["Message"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Login", "Login");
        }
    

    }
}
