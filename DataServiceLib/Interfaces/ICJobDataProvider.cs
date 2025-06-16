using CoreLib.Models;

namespace DataServiceLib.Interfaces
{
    public interface ICJobDataProvider
    {
        Task<List<Dictionary<string, object>>> AddJob(string id, string ten_job);
        Task<List<Dictionary<string, object>>> getall();
        Task<CResponseMessage> DeleteJob(string majob);
    }
}