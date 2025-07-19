using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.Dtos;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

public class OtpService : IOtpService
{
    private readonly IRedisService _redisService;
    private readonly IErrorHandler _errorHandler;
    private readonly SmtpSettings _smtpSettings;

    private const int CooldownSeconds = 30;
    private const int OtpExpireMinutes = 1;
    private const int MaxOtpKeyAliveMinutes = 30;
    private const int MaxFailCount = 5;

    public OtpService(IRedisService redisService, IErrorHandler errorHandler, IOptions<SmtpSettings> smtpSettings)
    {
        _redisService = redisService;
        _errorHandler = errorHandler;
        _smtpSettings = smtpSettings.Value;
    }

    public string GenerateOtp()
    {
        _errorHandler.WriteStringToFuncion(nameof(OtpService), nameof(GenerateOtp));
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task SendOtpEmail(string email, string otpCode)
    {
        _errorHandler.WriteStringToFuncion(nameof(OtpService), nameof(SendOtpEmail));
        try
        {
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var message = new MailMessage(_smtpSettings.Username, email)
            {
                Subject = "Mã OTP đăng nhập",
                Body = $"Mã OTP của bạn là: {otpCode}. Mã này có hiệu lực trong {OtpExpireMinutes} phút.",
                IsBodyHtml = false
            };

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _errorHandler.WriteToFile(ex);
            throw;
        }
    }

    public async Task<OtpSendStatus> SendOtpAndSaveToRedis(string email)
    {
        _errorHandler.WriteStringToFuncion(nameof(OtpService), nameof(SendOtpAndSaveToRedis));
        try
        {
            var lowerEmail = email.ToLower();
            var otpKey = $"otp:{lowerEmail}";
            var cooldownKey = $"{otpKey}:cooldown";

            // Kiểm tra cooldown 
            var isCooldown = await _redisService.ExistsAsync(cooldownKey);
            if (isCooldown)
            {
                return OtpSendStatus.CooldownActive;
            }

            var otp = GenerateOtp();

            //  OTP có hiệu lực trong 5 phút

            await _redisService.SetAsync(otpKey, otp, TimeSpan.FromMinutes(OtpExpireMinutes));

            //  cooldown 1 phút sau đó có thể gửi lại OTP
            await _redisService.SetAsync(cooldownKey, "1", TimeSpan.FromSeconds(CooldownSeconds));

            //  key không tồn tại quá 30 phút
            var ttl = await _redisService.GetTTLAsync(otpKey);
            if (ttl > TimeSpan.FromMinutes(MaxOtpKeyAliveMinutes))
            {
                await _redisService.ExpireAsync(otpKey, TimeSpan.FromMinutes(MaxOtpKeyAliveMinutes));
            }

            await SendOtpEmail(email, otp);
            return OtpSendStatus.Success;
        }
        catch (Exception ex)
        {
            _errorHandler.WriteToFile(ex);
            return OtpSendStatus.Failed;
        }
    }

    public async Task<OtpVerifyStatus> VerifyOtpWithLimit(string email, string inputOtp)
    {
        _errorHandler.WriteStringToFuncion(nameof(OtpService), nameof(VerifyOtpWithLimit));
        try
        {
            var lowerEmail = email.ToLower();
            var otpKey = $"otp:{lowerEmail}";
            var failKey = $"otp:fail:{lowerEmail}";

            var savedOtp = await _redisService.GetObjectAsync<string>(otpKey);

            if (string.IsNullOrWhiteSpace(savedOtp))
            {
                return OtpVerifyStatus.ExpiredOrInvalid;
            }

            if (savedOtp == inputOtp)
            {
                await _redisService.DeleteAsync(otpKey);
                await _redisService.DeleteAsync(failKey);
                return OtpVerifyStatus.Success;
            }

            //  tăng fail count
            var failCount = await _redisService.GetObjectAsync<int>(failKey);
            failCount++;
            // sai tối đa 5 lần
            if (failCount >= MaxFailCount)
            {
                await _redisService.DeleteAsync(otpKey);
                await _redisService.DeleteAsync(failKey);
                _errorHandler.WriteStringToFuncion("Nhập sai OTP quá 5 lần – Xoá OTP", "");
                return OtpVerifyStatus.TooManyAttempts;
            }

            // Cập nhật lại số lần sai
            await _redisService.SetAsync(failKey, failCount, TimeSpan.FromMinutes(MaxOtpKeyAliveMinutes));
            return OtpVerifyStatus.ExpiredOrInvalid;
        }
        catch (Exception ex)
        {
            _errorHandler.WriteToFile(ex);
            return OtpVerifyStatus.ExpiredOrInvalid;
        }
    }
}
