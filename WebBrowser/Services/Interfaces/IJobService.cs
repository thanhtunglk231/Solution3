using CoreLib.Models;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IJobService
    {
        Task<ApiResponse> Addjob(string id, string jobname, string token);
        Task<List<Job>> getall(string token);
        Task<ApiResponse?> DeleteJob(string majob, string token);
    }
}