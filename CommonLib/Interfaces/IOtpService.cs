using CoreLib.Dtos;

namespace CommonLib.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp();
        Task<OtpSendStatus> SendOtpAndSaveToRedis(string email);
        Task SendOtpEmail(string email, string otpCode);
        Task<OtpVerifyStatus> VerifyOtpWithLimit(string email, string inputOtp);
    }
}