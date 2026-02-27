using CommonLib.Interfaces;
using Microsoft.Extensions.Configuration;
using OtpNet;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonLib.Handles
{
    /// <summary>
    /// Dịch vụ TOTP dùng Otp.NET, cấu hình từ appsettings:
    /// "Mfa": { "TotpStepSeconds": 30, "TotpSkewSteps": 1, "Issuer": "MyWebApp" }
    /// </summary>
    public class TotpService : ITotpService
    {
        private readonly int _stepSeconds;
        private readonly int _skewSteps;
        private readonly string _issuer;

        public TotpService(IConfiguration configuration)
        {
            _stepSeconds = configuration.GetValue<int>("Mfa:TotpStepSeconds", 30);
            _skewSteps = configuration.GetValue<int>("Mfa:TotpSkewSteps", 1);
            _issuer = configuration.GetValue<string>("Mfa:Issuer") ?? "MyWebApp";
        }

        public string GenerateSecretBase32(int bytes = 20)
        {
            if (bytes < 16) bytes = 16; // an toàn tối thiểu
            var secret = KeyGeneration.GenerateRandomKey(bytes);
            return Base32Encoding.ToString(secret); // chuẩn Base32 Google Authenticator
        }

        public string BuildOtpAuthUrl(string issuer, string account, string secretBase32)
        {
            // issuer ưu tiên tham số truyền vào, nếu null dùng cấu hình
            var iss = string.IsNullOrWhiteSpace(issuer) ? _issuer : issuer.Trim();
            var acc = account?.Trim() ?? string.Empty;
            var sec = secretBase32?.Trim() ?? string.Empty;

            // otpauth://totp/Issuer:Account?secret=...&issuer=Issuer&digits=6&period=30&algorithm=SHA1
            return $"otpauth://totp/{Uri.EscapeDataString(iss)}:{Uri.EscapeDataString(acc)}" +
                   $"?secret={sec}&issuer={Uri.EscapeDataString(iss)}&digits=6&period={_stepSeconds}&algorithm=SHA1";
        }

        public bool Verify(string secretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(secretBase32)) return false;

            var normalized = NormalizeCode(code);
            if (normalized.Length != 6 || !normalized.All(char.IsDigit)) return false;

            byte[] secret;
            try { secret = Base32Encoding.ToBytes(secretBase32.Trim()); }
            catch { return false; }

            var totp = new Totp(secret, step: _stepSeconds, mode: OtpHashMode.Sha1, totpSize: 6);

            // chấp nhận lệch thời gian ±_skewSteps (mỗi step = _stepSeconds)
            return totp.VerifyTotp(normalized, out _, new VerificationWindow(_skewSteps, _skewSteps));
        }

        public string GetCurrentCode(string secretBase32)
        {
            if (string.IsNullOrWhiteSpace(secretBase32)) return string.Empty;
            byte[] secret = Base32Encoding.ToBytes(secretBase32.Trim());
            var totp = new Totp(secret, step: _stepSeconds, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.ComputeTotp(DateTime.UtcNow);
        }

        private static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            // bỏ khoảng trắng, dấu gạch, ký tự không phải số
            return Regex.Replace(code, "[^0-9]", string.Empty).Trim();
        }
    }
}
