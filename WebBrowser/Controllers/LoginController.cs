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

            // Kiểm tra null để tránh lỗi NullReferenceException
            if (loginResult != null)
            {
                TempData["Message"] = loginResult.Message;
                HttpContext.Session.SetString("Token", loginResult.token);
                return RedirectToAction("GetAll", "Employee");
            }
            else
            {
                TempData["Message"] = "Không nhận được phản hồi từ server.";
            }

            return View();
        }


    }
}
