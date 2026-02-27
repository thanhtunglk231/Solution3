namespace CommonLib.Handles
{
    public interface ITotpService
    {
        string BuildOtpAuthUrl(string issuer, string account, string secretBase32);
        string GenerateSecretBase32(int bytes = 20);
        string GetCurrentCode(string secretBase32);
        bool Verify(string secretBase32, string code);
    }
}