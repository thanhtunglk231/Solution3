using CoreLib.Models;

namespace DataServiceLib.Interfaces1
{
    public interface ICLoginProvider
    {
        
     
        Task<CResponseMessage1> Login(string username, string password);
        Task<CResponseMessage1> Register(string username, string password);
      
    }
}