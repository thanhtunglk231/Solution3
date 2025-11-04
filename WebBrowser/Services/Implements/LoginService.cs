using CommonLib.Handles;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using Microsoft.Extensions.Configuration;
using WebBrowser.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class LoginService : ILoginService
    {
        private readonly IHttpService _httpService;
        private readonly IErrorHandler _errorHandler;
        private readonly string _baseUrl;

        public LoginService(IHttpService httpService, IConfiguration configuration, IErrorHandler errorHandler)
        {
            _httpService = httpService;
            _errorHandler = errorHandler;
            _baseUrl = configuration["PathStrings:Url"] ?? "";
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(LoginAsync));

                var loginRequest = new LoginRequest
                {
                    username = username,
                    password = password
                };

                var url = _baseUrl + ApiRouter.Login;

                var result = await _httpService.PostAsync<LoginResponse>(url, loginRequest);

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new LoginResponse
                {
                    Code = "500",
                    Message = "Lỗi khi đăng nhập.",
                    Success = false
                };
            }
        }


        public async Task<LoginResponse?> SendOtp(InputStringDto input)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(SendOtp));

                var url = _baseUrl + "login/send";

              
                var result = await _httpService.PostAsync<LoginResponse>(url, input);

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new LoginResponse
                {
                    Code = "500",
                    Message = "Lỗi gửi OTP",
                    Success = false
                };
            }
        }
        public async Task<LoginResponse?> VerifyOtp(VerifyOtpRequest verifyOtp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(VerifyOtp));

                var url = _baseUrl + "login/verify-otp";

             
                var result = await _httpService.PostAsync<LoginResponse>(url, verifyOtp);

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new LoginResponse
                {
                    Code = "500",
                    Message = "Lỗi xác thực OTP",
                    Success = false
                };
            }
        }


        public async Task<LoginResponse?> Register(RegisterDto registerDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(Register));

                var url = _baseUrl + ApiRouter.Register;

                var result = await _httpService.PostAsync<LoginResponse>(url, registerDto);

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new LoginResponse
                {
                    Code = "500",
                    Message = "Lỗi khi đăng ký tài khoản.",
                    Success = false
                };
            }
        }


        public async Task<TotpEnrollStartResponse?> TotpEnrollStartAsync(string username, string issuer = "MyWebApp")
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(TotpEnrollStartAsync));
                var url = _baseUrl + ApiRouter.TotpEnrollStart;

                var req = new TotpEnrollStartRequest { username = username, issuer = issuer };
                return await _httpService.PostAsync<TotpEnrollStartResponse>(url, req);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new TotpEnrollStartResponse { success = false, message = "Lỗi tạo QR TOTP." };
            }
        }

        public async Task<SimpleResponse?> TotpEnrollConfirmAsync(string username, string secretBase32, string code)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(TotpEnrollConfirmAsync));
                var url = _baseUrl + ApiRouter.TotpEnrollConfirm;

                var req = new TotpEnrollConfirmRequest { username = username, secretBase32 = secretBase32, code = code };
                return await _httpService.PostAsync<SimpleResponse>(url, req);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new SimpleResponse { success = false, message = "Lỗi xác nhận bật TOTP." };
            }
        }

        public async Task<LoginResponse?> TotpVerifyAsync(string username, string code)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(TotpVerifyAsync));
                var url = _baseUrl + ApiRouter.TotpVerify;

                var req = new TotpVerifyRequest { username = username, code = code };
                return await _httpService.PostAsync<LoginResponse>(url, req);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new LoginResponse { Code = "500", Message = "Lỗi xác minh TOTP.", Success = false };
            }
        }


    }
}
