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

    }
}
