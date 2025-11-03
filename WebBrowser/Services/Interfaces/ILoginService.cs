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

        Task<LoginResponse?> TotpVerifyAsync(string username, string code);
        Task<SimpleResponse?> TotpEnrollConfirmAsync(string username, string secretBase32, string code);
        Task<TotpEnrollStartResponse?> TotpEnrollStartAsync(string username, string issuer = "MyWebApp");
        //Task<ApiResponse?> UpdateUserAsync(string username, string password, string role, string manv);
    }
}