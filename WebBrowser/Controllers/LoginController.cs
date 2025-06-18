using CoreLib.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Models;
using WebBrowser.Models.ViewModels;
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
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var loginResult = await _loginService.LoginAsync(model.Username, model.Password);

            if (loginResult != null)
            {
                HttpContext.Session.SetString("JWToken", loginResult.Token ?? "");
                HttpContext.Session.SetString("Username", loginResult.Username ?? "");
                HttpContext.Session.SetString("Role", loginResult.Role ?? "");
                if (loginResult.Success && !string.IsNullOrEmpty(loginResult.Token))
                {
                    HttpContext.Session.SetString("JWToken", loginResult.Token);
                    model.Response = new ApiResponse
                    {
                        Success = true,
                        Message = loginResult.Message
                    };
                    ViewBag.RedirectUrl = Url.Action("GetAll", "Employee");
                    return View(model);
                }
                else
                {
                    model.Response = new ApiResponse
                    {
                        Success = false,
                        Message = loginResult.Message
                    };
                }
            }
            else
            {
                model.Response = new ApiResponse
                {
                    Success = false,
                    Message = "Không nhận được phản hồi từ server."
                };
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var apiResult = await _loginService.getall();

            var viewModel = new UserListViewModel
            {
                Users = apiResult?.data ?? new List<UserDto>(),
                Response = new ApiResponse
                {
                    Success = apiResult?.success ?? false,
                    Message = apiResult?.message ?? "Không lấy được dữ liệu từ server."
                }
            };

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginViewModel model)
        {
            var result = await _loginService.Register(model.Username, model.Password);

            model.Response = new ApiResponse
            {
                Message = result.Message,
                Success = result.Success
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout(LoginViewModel model)
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Admin");
            model.Response = new ApiResponse
            {
                Message = "Bạn đã đăng xuất",
                
            };
            return RedirectToAction("Login", "Login");
        }


    }
}