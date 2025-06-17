using DataServiceLib.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Reflection;
namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICLoginProvider _userService;
        private readonly IConfiguration _configuration;
        public LoginController(ICLoginProvider userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CoreLib.Dtos.LoginRequest req)
        {
            var result = await _userService.Login(req.username, req.password);


            if (result == null || result.code != "200")
            {
                return Ok(new
                {
                    code = result?.code,
                    message = result?.message ?? "Lỗi không xác định.",
                    success = false,
                    token = (string?)null,
                    username = req.username,
                    role = (string?)null,
                    manv = (string?)null
                });
            }

            var userData = result.Data?.FirstOrDefault();
            var role = userData?["role"]?.ToString();
            var manv = userData?["manv"]?.ToString();

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CoreLib.Dtos.LoginRequest loginRequest)
        {
            var result = await _userService.Register(loginRequest.username, loginRequest.password);
            return Ok(result);
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