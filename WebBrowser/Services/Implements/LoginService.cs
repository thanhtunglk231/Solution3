using CommonLib.Handles;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using Microsoft.Extensions.Configuration;
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

        public async Task<LoginResponse?> Register(string username, string password)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(LoginService), nameof(Register));

                var request = new LoginRequest
                {
                    username = username,
                    password = password
                };

                var url = _baseUrl + ApiRouter.Register;

                var result = await _httpService.PostAsync<LoginResponse>(url, request);

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
