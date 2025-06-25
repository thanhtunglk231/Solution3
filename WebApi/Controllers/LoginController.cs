using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreLib.Dtos;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using CommonLib.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICLoginProvider _userService;
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler _errorHandler;

        public LoginController(ICLoginProvider userService, IConfiguration configuration, IErrorHandler errorHandler)
        {
            _userService = userService;
            _configuration = configuration;
            _errorHandler = errorHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(Login));

                if (req == null || string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.password))
                {
                    return BadRequest(new
                    {
                        code = "400",
                        message = "Thông tin đăng nhập không hợp lệ.",
                        success = false
                    });
                }

                var result = await _userService.Login(req.username, req.password);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        code = "500",
                        message = "Không có phản hồi từ dịch vụ đăng nhập.",
                        success = false,
                        token = (string?)null,
                        username = req.username,
                        role = (string?)null,
                        manv = (string?)null
                    });
                }

                var dataList = result.Data as List<Dictionary<string, object>>;

                if (result.code != "200" || dataList == null || !dataList.Any())
                {
                    return Ok(new
                    {
                        code = result.code ?? "401",
                        message = result.message ?? "Tài khoản hoặc mật khẩu không đúng.",
                        success = false,
                        token = (string?)null,
                        username = req.username,
                        role = (string?)null,
                        manv = (string?)null
                    });
                }

                var userData = dataList.FirstOrDefault() ?? new Dictionary<string, object>();
                var role = userData.ContainsKey("role") ? userData["role"]?.ToString() : null;
                var manv = userData.ContainsKey("manv") ? userData["manv"]?.ToString() : null;

                var token = GenerateJwtToken(req.username, role, manv);

                return Ok(new
                {
                    code = result.code,
                    message = result.message,
                    success = true,
                    token,
                    username = req.username,
                    role,
                    manv
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new
                {
                    code = "500",
                    message = "Đã xảy ra lỗi trong quá trình đăng nhập.",
                    detail = ex.Message,
                    success = false
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest loginRequest)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "register");
                var result = await _userService.Register(loginRequest.username, loginRequest.password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler?.WriteToFile(ex);
                return StatusCode(500, new
                {
                    code = "500",
                    message = "Đã xảy ra lỗi trong quá trình đăng ký.",
                    detail = ex.Message,
                    success = false
                });
            }
        }

        private string GenerateJwtToken(string username, string role, string manv)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role ?? ""),
                new Claim("manv", manv ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
