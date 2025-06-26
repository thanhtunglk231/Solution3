using CommonLib.Handles;
using CoreLib.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Models;
using WebBrowser.Models.ViewModels;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ILoginService _loginService;
        private readonly IErrorHandler _errorHandler;
        public LoginController(ILoginService loginService, IConfiguration configuration,IErrorHandler errorHandler)
            : base(configuration)
        {
            _loginService = loginService;
            _errorHandler = errorHandler;   
        }

        // Hiển thị giao diện đăng nhập
        public IActionResult Index()
        {
            return View();
        }
       
       
        public async Task<IActionResult> LoginJson([FromBody] LoginViewModel model)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "LoginJson");
                var loginResult = await _loginService.LoginAsync(model.Username, model.Password);

                if (loginResult != null && loginResult.Success && !string.IsNullOrEmpty(loginResult.Token))
                {
                    HttpContext.Session.SetString("JWToken", loginResult.Token);
                    HttpContext.Session.SetString("Username", loginResult.Username ?? "");
                    HttpContext.Session.SetString("Role", loginResult.Role ?? "");

                    return Json(new
                    {
                        success = true,
                        message = loginResult.Message,
                        token = loginResult.Token,
                        role = loginResult.Role,
                        username = loginResult.Username
                    });
                }

                return Json(new
                {
                    success = false,
                    message = loginResult?.Message ?? "Đăng nhập thất bại"
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }

       


       
        public async Task<IActionResult> RegisterJson([FromBody] LoginViewModel model)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "RegisterJson");
                var result = await _loginService.Register(model.Username, model.Password);

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        username = model.Username
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }


        #region
        // Cập nhật người dùng (chỉ Admin mới được phép)
        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(LoginViewModel model)
        //{
        //    if (!IsAdmin())
        //        return RedirectNoPermission("Không có quyền chỉnh sửa");

        //    //var response = await _loginService.UpdateUserAsync(model.Username, model.Password, model.Role, model.Manv);

        //    TempData["Message"] = response.Message;
        //    TempData["Success"] = response.Success;

        //    return RedirectToAction("GetAll");
        //}
        #endregion

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout(LoginViewModel model)
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Username");

            model.Response = new ApiResponse
            {
                Message = "Bạn đã đăng xuất"
            };

            return RedirectToAction("Index");
        }
    }
}
