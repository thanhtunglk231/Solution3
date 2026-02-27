using CoreLib.Models;

namespace DataServiceLib.unuse.Interfaces.unuse
{
    public interface ICJobDataProvider
    {
        Task<List<Dictionary<string, object>>> AddJob(string id, string ten_job);
        Task<List<Dictionary<string, object>>> getall();
        Task<CResponseMessage> DeleteJob(string majob);
    }
}