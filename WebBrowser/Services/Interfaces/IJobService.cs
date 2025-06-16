using CoreLib.Models;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IJobService
    {
        Task<ApiResponse> Addjob(string id, string jobname);
        Task<List<Job>> getall();
        Task<ApiResponse?> DeleteJob(string majob);
    }
}