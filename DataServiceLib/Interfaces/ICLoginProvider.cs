using CoreLib.Models;

namespace DataServiceLib.Implementations
{
    public interface ICLoginProvider
    {
        Task<CResponseMessage> Login(string username, string password);
        Task<CResponseMessage> Register(string username, string password);
        Task<CResponseMessage> Getall();
    }
}