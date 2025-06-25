using CoreLib.Models;

namespace DataServiceLib.Interfaces1
{
    public interface ICLoginProvider
    {
        Task<CResponseMessage1> Getall();
        Task<(List<Dictionary<string, object>> data, CResponseMessage1 response)> GetPermission();
        Task<CResponseMessage1> Login(string username, string password);
        Task<CResponseMessage1> Register(string username, string password);
        Task<CResponseMessage1> UpdateUserInfo(string username, string newPassword, string newRole, string newManv);
    }
}