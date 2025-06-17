using Microsoft.AspNetCore.Mvc;

namespace WebBrowser.Controllers
{
    public class BaseController : Controller
    {
        protected string? GetJwtTokenOrRedirect(out IActionResult? redirectResult)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["message"] = "Bạn cần đăng nhập";
                redirectResult = RedirectToAction("Login", "Login");
                return null;
            }
            redirectResult = null;
            return token;
        }
    }
}
