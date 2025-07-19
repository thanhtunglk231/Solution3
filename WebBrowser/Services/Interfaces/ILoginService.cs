using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponse?> LoginAsync(string username, string password);
        Task<LoginResponse?> Register(RegisterDto registerDto); 
        Task<LoginResponse?> SendOtp(InputStringDto input);
        Task<LoginResponse?> VerifyOtp(VerifyOtpRequest verifyOtp);


        //Task<ApiResponse?> UpdateUserAsync(string username, string password, string role, string manv);
    }
}