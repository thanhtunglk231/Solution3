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
