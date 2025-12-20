using CommonLib.Handles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class MyProfileController : BaseController
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IMyProfileService _myProfileService;

        public MyProfileController(IErrorHandler errorHandler, IMyProfileService myProfileService, IConfiguration configuration)
            : base(configuration)
        {
            _errorHandler = errorHandler;
            _myProfileService = myProfileService;
        }

        public IActionResult Index()
        {
            try
            {
                Console.WriteLine("[MyProfileController][Index] Called");

                var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
                Console.WriteLine("[MyProfileController][Index] Token: " + (token == null ? "NULL" : "EXISTS"));

                if (token == null) return redirectResult!;
                Console.WriteLine("[MyProfileController][Index] Return View()");
                return View();
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return View();
            }
        }

        // GET: /MyProfile/Me
        [HttpGet]
        public async Task<IActionResult> Me()
        {
            Console.WriteLine("[MyProfileController][Me] START");
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileController), nameof(Me));
                Console.WriteLine("[MyProfileController][Me] Logged to _errorHandler");

                var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
                Console.WriteLine("[MyProfileController][Me] Token: " + (token == null ? "NULL" : token.Substring(0, Math.Min(20, token.Length)) + "..."));

                if (token == null) return redirectResult!;

                _myProfileService.UseToken(token);
                Console.WriteLine("[MyProfileController][Me] Set token to MyProfileService");

                var result = await _myProfileService.GetMe();

                Console.WriteLine("[MyProfileController][Me] Result:");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

                Console.WriteLine("[MyProfileController][Me] END");
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine("[MyProfileController][Me] ERROR: " + ex);
                return Json(new { success = false, code = "500", message = "Lỗi khi lấy thông tin cá nhân." });
            }
        }

        // POST: /MyProfile/Update
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateMyProfileRequest model)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileController), nameof(Update));

                var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
                if (token == null) return redirectResult!;

                _myProfileService.UseToken(token);

                var result = await _myProfileService.UpdateMe(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, code = "500", message = "Lỗi khi cập nhật thông tin cá nhân." });
            }
        }

        // POST: /MyProfile/DeleteMe
        [HttpPost]
        public async Task<IActionResult> DeleteMe()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileController), nameof(DeleteMe));

                var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
                if (token == null) return redirectResult!;

                _myProfileService.UseToken(token);

                var result = await _myProfileService.DeleteMe();
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, code = "500", message = "Lỗi khi xóa tài khoản." });
            }
        }
    }
}
