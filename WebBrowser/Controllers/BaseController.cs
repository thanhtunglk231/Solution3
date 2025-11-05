
using CoreLib.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebBrowser.Controllers
{
    public class BaseController : Controller
    {
        private readonly IConfiguration _configuration;

        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected string? GetRole()
        {
            return HttpContext.Session.GetString("Role");
        }

        protected bool IsAdmin()
        {
            return GetRole() == "Admin";
        }

        protected IActionResult RedirectNoPermission(string? message = null)
        {
            message ??= "Bạn không có quyền truy cập chức năng này.";
            return RedirectToAction("Getall", new { msg = message, success = false });
        }
        protected string? GetJwtTokenOrRedirect(out IActionResult? redirectResult)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["message"] = "Bạn cần đăng nhập";
                redirectResult = RedirectToAction("Index", "Login");
                return null;
            }
            redirectResult = null;
            return token;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                var secretKey = _configuration["JwtSettings:SecretKey"];
           

                var principal = JwtHelper.GetPrincipalFromToken(token, secretKey);

                if (principal != null)
                {

                    ViewBag.Username = principal.Identity?.Name;
                    ViewBag.Role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    ViewBag.Manv = principal.Claims.FirstOrDefault(c => c.Type == "manv")?.Value;
                    ViewBag.IsLoggedIn = true;
                }
                else
                {
                   
                    ViewBag.IsLoggedIn = false;
                }
            }
            else
            {
                
                ViewBag.IsLoggedIn = false;
            }

            base.OnActionExecuting(context);
        }
    }
}
