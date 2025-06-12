using WebBrowser.Models;

namespace WebBrowser.Interfaces
{
    public interface IJobService
    {
        Task<ApiResponse> Addjob(string id, string jobname);
    }
}