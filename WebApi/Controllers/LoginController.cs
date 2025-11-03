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
using CommonLib.Interfaces;
using WebBrowser.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICLoginProvider _userService;
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler _errorHandler;
        private readonly IOtpService _otpService;
        private readonly ITotpService _totpService;
        public LoginController(ICLoginProvider userService, ITotpService totpService, IConfiguration configuration, IErrorHandler errorHandler,IOtpService otpService)
        {
            _userService = userService;
            _configuration = configuration;
            _errorHandler = errorHandler;
            _totpService = totpService;
            _otpService = otpService;   
        }
        // POST /api/login
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

                var result = await _userService.Login(req.username.Trim(), req.password.Trim());

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        code = "500",
                        message = "Không có phản hồi từ dịch vụ đăng nhập.",
                        success = false
                    });
                }

                // Sai mật khẩu / lỗi dịch vụ
                var dataList = result.Data as List<Dictionary<string, object>>;
                if (result.code != "200" || dataList == null || !dataList.Any())
                {
                    return Ok(new
                    {
                        code = result.code ?? "401",
                        message = result.message ?? "Tài khoản hoặc mật khẩu không đúng.",
                        success = false
                    });
                }

                // Đúng mật khẩu → lấy thông tin cơ bản
                var userData = dataList.First();
                var role = userData.TryGetValue("role", out var _role) ? _role?.ToString() : null;
                var manv = userData.TryGetValue("manv", out var _manv) ? _manv?.ToString() : null;
                var email = userData.TryGetValue("email", out var _email) ? _email?.ToString() : null;

                // Kiểm tra trạng thái TOTP
                var (isEnabled, _, lockedUntil) = await _userService.GetTotpInfoAsync(req.username.Trim());

                if (lockedUntil.HasValue && lockedUntil.Value > DateTimeOffset.UtcNow)
                {
                    return Unauthorized(new
                    {
                        code = "401",
                        message = $"Tài khoản đang bị khóa MFA đến {lockedUntil.Value:HH:mm:ss dd/MM/yyyy}.",
                        success = false
                    });
                }

                if (isEnabled)
                {
                    // CHÚ Ý: Không cấp token ở đây. Yêu cầu người dùng nhập mã TOTP.
                    return Ok(new
                    {
                        code = "200",
                        message = "Mật khẩu đúng. Vui lòng nhập mã Google Authenticator.",
                        success = true,
                        mfaRequired = true,
                        methods = new[] { "totp" },
                        username = req.username, // FE sẽ dùng lại
                        role,
                        manv,
                        email
                    });
                }

                // Chưa bật TOTP → cấp token luôn
                var token = GenerateJwtToken(req.username, role, manv, email);
                return Ok(new
                {
                    code = "200",
                    message = "Đăng nhập thành công.",
                    success = true,
                    token,
                    username = req.username,
                    role,
                    manv,
                    email
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
        public async Task<IActionResult> Register([FromBody] RegisterDto loginRequest)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("LoginController", "register");
                var result = await _userService.Register(loginRequest);
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
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] InputStringDto input)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(Send));

                var response = await _userService.GetEmail(input.Input.Trim());

                if (!response.Success || response.Data == null)
                {
                    _errorHandler.WriteStringToFuncion("Không tìm thấy email cho input: {Input}", input.Input);
                    return BadRequest(new { success = false, message = "Không tìm thấy email." });
                }

                var emailProp = response.Data.GetType().GetProperty("email");
                var email = emailProp?.GetValue(response.Data)?.ToString();

                if (string.IsNullOrWhiteSpace(email))
                {
                    _errorHandler.WriteStringToFuncion("Email rỗng hoặc không hợp lệ từ input: {Input}", input.Input);
                    return BadRequest(new { success = false, message = "Email không hợp lệ." });
                }

                _errorHandler.WriteStringToFuncion("Đang gửi OTP đến email: {Email}", email);
                var otpResult = await _otpService.SendOtpAndSaveToRedis(email);

                switch (otpResult)
                {
                    case OtpSendStatus.Success:
                        _errorHandler.WriteStringToFuncion("Đã gửi OTP thành công cho email: {Email}", email);
                        return Ok(new { success = true, message = "OTP đã được gửi tới email.", email });

                    case OtpSendStatus.CooldownActive:
                        return BadRequest(new
                        {
                            success = false,
                            message = "OTP đã được gửi gần đây. Vui lòng chờ 1 phút trước khi gửi lại."
                        });

                    case OtpSendStatus.Failed:
                    default:
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Gửi OTP thất bại. Vui lòng thử lại sau."
                        });
                }
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi gửi OTP." });
            }
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(VerifyOtp));

                if (string.IsNullOrWhiteSpace(req.email) || string.IsNullOrWhiteSpace(req.otp))
                {
                    return BadRequest(new
                    {
                        code = "400",
                        message = "Email hoặc mã OTP không hợp lệ.",
                        success = false
                    });
                }

                var result = await _otpService.VerifyOtpWithLimit(req.email.Trim(), req.otp.Trim());

                switch (result)
                {
                    case OtpVerifyStatus.Success:
                        break; 

                    case OtpVerifyStatus.TooManyAttempts:
                        return Unauthorized(new
                        {
                            code = "401",
                            message = "Bạn đã nhập sai OTP quá 5 lần. Vui lòng yêu cầu gửi mã mới.",
                            success = false
                        });

                    case OtpVerifyStatus.ExpiredOrInvalid:
                    default:
                        return Unauthorized(new
                        {
                            code = "401",
                            message = "Mã OTP không đúng hoặc đã hết hạn.",
                            success = false
                        });
                }

                var response = await _userService.GetEmail(req.email.Trim());

                if (!response.Success || response.Data == null)
                {
                    return NotFound(new
                    {
                        code = "404",
                        message = "Người dùng không tồn tại.",
                        success = false
                    });
                }

                var data = response.Data;
                var email = data.GetType().GetProperty("email")?.GetValue(data)?.ToString();
                var username = data.GetType().GetProperty("username")?.GetValue(data)?.ToString();
                var role = data.GetType().GetProperty("role")?.GetValue(data)?.ToString();
                var manv = data.GetType().GetProperty("manv")?.GetValue(data)?.ToString();

                var token = GenerateJwtToken(username, role, manv, email);

                return Ok(new
                {
                    code = "200",
                    message = "Xác thực OTP thành công.",
                    success = true,
                    token,
                    username,
                    role,
                    manv,
                    email
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new
                {
                    code = "500",
                    message = "Lỗi hệ thống khi xác thực OTP.",
                    detail = ex.Message,
                    success = false
                });
            }
        }



        // totp google authencator
        [HttpPost("totp/enroll-start")]
        public async Task<IActionResult> TotpEnrollStart([FromBody] TotpEnrollStartRequest req)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(TotpEnrollStart));

                if (string.IsNullOrWhiteSpace(req.username))
                    return BadRequest(new { success = false, message = "Thiếu username." });

                // 🔹 Kiểm tra user có tồn tại trong hệ thống không
                var userInfo = await _userService.GetEmail(req.username.Trim());
                if (!userInfo.Success || userInfo.Data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Tài khoản không tồn tại hoặc chưa được đăng ký."
                    });
                }

                // 🔹 Kiểm tra đã bật MFA chưa
                var (enabled, _, _) = await _userService.GetTotpInfoAsync(req.username.Trim());
                if (enabled)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tài khoản đã bật xác thực Google Authenticator."
                    });
                }

                // 🔹 Sinh mã secret + QR
                var issuer = string.IsNullOrWhiteSpace(req.issuer) ? "MyWebApp" : req.issuer.Trim();
                var secret = _totpService.GenerateSecretBase32();
                var otpauth = _totpService.BuildOtpAuthUrl(issuer, req.username.Trim(), secret);
                var qrPng = QrHelper.ToBase64Png(otpauth);

                return Ok(new
                {
                    success = true,
                    message = "Quét QR này trong Google Authenticator để thêm tài khoản.",
                    secretBase32 = secret,
                    otpauthUrl = otpauth,
                    qrPngBase64 = qrPng
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi tạo mã QR TOTP.",
                    detail = ex.Message
                });
            }
        }





        [HttpPost("totp/enroll-confirm")]
        public async Task<IActionResult> TotpEnrollConfirm([FromBody] TotpEnrollConfirmRequest req)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(TotpEnrollConfirm));

                if (string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.code))
                    return BadRequest(new { success = false, message = "Thiếu thông tin xác nhận." });

                bool verified = _totpService.Verify(req.secretBase32, req.code);

                if (!verified)
                    return Unauthorized(new { success = false, message = "Mã TOTP không đúng hoặc đã hết hạn." });

                await _userService.EnableTotpAsync(req.username, req.secretBase32);

                return Ok(new
                {
                    success = true,
                    message = "Đã kích hoạt xác thực Google Authenticator cho tài khoản."
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new { success = false, message = "Lỗi khi xác nhận TOTP." });
            }
        }






        [HttpPost("totp/verify")]
        public async Task<IActionResult> TotpVerify([FromBody] TotpVerifyRequest req)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginController), nameof(TotpVerify));

                if (string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.code))
                    return BadRequest(new { success = false, message = "Thiếu username hoặc mã TOTP." });

                var (isEnabled, secret, lockedUntil) = await _userService.GetTotpInfoAsync(req.username);

                if (!isEnabled)
                    return BadRequest(new { success = false, message = "Tài khoản chưa bật xác thực Google Authenticator." });

                if (lockedUntil.HasValue && lockedUntil.Value > DateTimeOffset.UtcNow)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = $"Tài khoản bị khoá tạm đến {lockedUntil.Value:HH:mm:ss dd/MM/yyyy}"
                    });
                }

                bool valid = _totpService.Verify(secret, req.code);

                if (!valid)
                {
                    var (count, newLock) = await _userService.TotpFailIncreaseAsync(req.username);
                    string msg = newLock.HasValue
                        ? $"Nhập sai quá nhiều. Tài khoản bị khoá đến {newLock.Value:HH:mm:ss dd/MM/yyyy}"
                        : $"Mã TOTP không đúng. Lần sai thứ {count}/5.";
                    return Unauthorized(new { success = false, message = msg });
                }

                await _userService.TotpSuccessAsync(req.username);

                // lấy info người dùng (giống login)
                var loginResult = await _userService.GetEmail(req.username);
                var data = loginResult?.Data;
                var email = data?.GetType().GetProperty("email")?.GetValue(data)?.ToString();
                var role = data?.GetType().GetProperty("role")?.GetValue(data)?.ToString();
                var manv = data?.GetType().GetProperty("manv")?.GetValue(data)?.ToString();

                var token = GenerateJwtToken(req.username, role, manv, email);

                return Ok(new
                {
                    success = true,
                    message = "Xác thực TOTP thành công.",
                    token,
                    username = req.username,
                    role,
                    manv,
                    email
                });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi xác minh TOTP." });
            }
        }















        private string GenerateJwtToken(string username, string role, string manv, string email)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username ?? ""),
                new Claim(ClaimTypes.Role, role ?? ""),
                new Claim("manv", manv ?? ""),
                new Claim(ClaimTypes.Email, email ?? "")
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
