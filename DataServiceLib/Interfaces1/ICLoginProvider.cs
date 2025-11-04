using CoreLib.Dtos;
using CoreLib.Models;

namespace DataServiceLib.Interfaces1
{
    public interface ICLoginProvider
    {
        
     
        Task<CResponseMessage1> Login(string username, string password);
        Task<CResponseMessage1> Register(RegisterDto dto);
        Task<CResponseMessage1> GetEmail(string input);
        Task<(bool IsEnabled, string SecretBase32, DateTimeOffset? LockedUntil)> GetTotpInfoAsync(string username);
        Task EnableTotpAsync(string username, string secretBase32);
        Task TotpSuccessAsync(string username);
        Task<(int FailCount, DateTimeOffset? LockedUntil)> TotpFailIncreaseAsync(string username, int maxAttempts = 5, int lockMinutes = 10);

    }
}