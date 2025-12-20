using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using DataServiceLib.Implementations1;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // bắt buộc đăng nhập JWT
    public class MyproFileController : ControllerBase
    {
        private readonly CMyproFile _myProfileService;
        private readonly IConfiguration _config;

        public MyproFileController(CMyproFile myProfileService, IConfiguration config)
        {
            _myProfileService = myProfileService;
            _config = config;
        }

        // =========================
        // GET: api/MyproFile/me
        // =========================
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var username = GetUsername();
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized(new
                {
                    success = false,
                    code = "401",
                    message = "Không xác định được người dùng."
                });

            // ✅ FIX: dùng đúng OracleDb
            var connectString = _config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectString))
                return StatusCode(500, new
                {
                    success = false,
                    code = "500",
                    message = "Thiếu ConnectionStrings:OracleDb trong appsettings.json"
                });

            var (data, response) = await _myProfileService.GetMyProfile(username, connectString);

            if (response == null || response.code != "200")
                return StatusCode(500, new
                {
                    success = false,
                    code = response?.code ?? "500",
                    message = response?.message ?? "Lỗi khi lấy thông tin cá nhân."
                });

            if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                return NotFound(new
                {
                    success = false,
                    code = "404",
                    message = "Không tìm thấy thông tin cá nhân."
                });

            return Ok(new
            {
                success = true,
                code = response.code,
                message = response.message,
                data = data.Tables[0]
            });
        }

        // =========================
        // PUT: api/MyproFile/me
        // =========================
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest model)
        {
            var username = GetUsername();
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized(new
                {
                    success = false,
                    code = "401",
                    message = "Không xác định được người dùng."
                });

            var connectString = _config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectString))
                return StatusCode(500, new
                {
                    success = false,
                    code = "500",
                    message = "Thiếu ConnectionStrings:OracleDb trong appsettings.json"
                });

            var res = await _myProfileService.UpdateMyProfile(
                username,
                model.Email,
                model.Sdt,
                model.Manv,
                connectString
            );

            if (res == null || res.code != "200")
                return BadRequest(new
                {
                    success = false,
                    code = res?.code ?? "400",
                    message = res?.message ?? "Cập nhật thất bại."
                });

            return Ok(new
            {
                success = true,
                code = res.code,
                message = res.message
            });
        }

        // =========================
        // DELETE: api/MyproFile/me
        // =========================
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var username = GetUsername();
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized(new
                {
                    success = false,
                    code = "401",
                    message = "Không xác định được người dùng."
                });

            var connectString = _config.GetConnectionString("OracleDb");
            if (string.IsNullOrWhiteSpace(connectString))
                return StatusCode(500, new
                {
                    success = false,
                    code = "500",
                    message = "Thiếu ConnectionStrings:OracleDb trong appsettings.json"
                });

            var res = await _myProfileService.DeleteMyAccount(username, connectString);

            if (res == null || res.code != "200")
                return BadRequest(new
                {
                    success = false,
                    code = res?.code ?? "400",
                    message = res?.message ?? "Xóa tài khoản thất bại."
                });

            return Ok(new
            {
                success = true,
                code = res.code,
                message = "Tài khoản đã bị xóa. Vui lòng đăng xuất."
            });
        }

        // =========================
        // Helper: lấy username từ JWT
        // =========================
        private string? GetUsername()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.FindFirst("username")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

    // =========================
    // DTO
    // =========================
    public class UpdateMyProfileRequest
    {
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public string? Manv { get; set; }
    }
}
