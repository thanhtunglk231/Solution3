using CoreLib.Dtos;
using CoreLib.Models;

namespace DataServiceLib.Interfaces1
{
    public interface ICLoginProvider
    {
        
     
        Task<CResponseMessage1> Login(string username, string password);
        Task<CResponseMessage1> Register(RegisterDto dto);
        Task<CResponseMessage1> GetEmail(string input);


    }
}