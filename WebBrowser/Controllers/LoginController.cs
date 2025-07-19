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


        
        public async Task<IActionResult> Send([FromBody] InputStringDto input)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "Send");
                Console.WriteLine($"[DEBUG] Bắt đầu Send OTP với input: {input}");

                var result = await _loginService.SendOtp(input);

                Console.WriteLine($"[DEBUG] Kết quả từ SendOtp: Success = {result?.Success}, Message = {result?.Message}, Email = {result?.email}");

                if (result != null && result.Success)
                {
                    if (!string.IsNullOrWhiteSpace(result.email))
                    {
                        HttpContext.Session.SetString("email", result.email);
                        Console.WriteLine($"[DEBUG] Đã lưu email vào session: {result.email}");
                    }

                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        email = result.email
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result?.Message ?? "Không thể gửi OTP."
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine($"[ERROR] Lỗi trong Send OTP: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống khi gửi OTP: " + ex.Message
                });
            }
        }



        // Xác thực OTP

   
        public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest loginRequest)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "Verify");
                Console.WriteLine($"[DEBUG] Bắt đầu Verify OTP với email: {loginRequest.email}, otp: {loginRequest.otp}");

                var result = await _loginService.VerifyOtp(loginRequest);

                Console.WriteLine($"[DEBUG] Kết quả VerifyOtp: Success = {result?.Success}, Token = {result?.Token}, Message = {result?.Message}");

                if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    HttpContext.Session.SetString("JWToken", result.Token);
                    HttpContext.Session.SetString("Username", result.Username ?? "");
                    HttpContext.Session.SetString("Role", result.Role ?? "");
                    HttpContext.Session.SetString("email", result.email ?? "");
                    HttpContext.Session.SetString("manv", result.Manv ?? "");

                    Console.WriteLine("[DEBUG] Lưu session thành công");

                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        token = result.Token,
                        username = result.Username,
                        role = result.Role,
                        manv = result.Manv,
                        email = result.email
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result?.Message ?? "Xác thực OTP thất bại."
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine($"[ERROR] Lỗi trong Verify OTP: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }






        public async Task<IActionResult> RegisterJson([FromBody] RegisterDto model)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "RegisterJson");

                var result = await _loginService.Register(model);

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        username = model.username,
                        email = model.email
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
