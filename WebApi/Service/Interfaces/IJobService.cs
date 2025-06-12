namespace WebApi.Service.Interfaces
{
    public interface IJobService
    {
        Task<List<Dictionary<string, object>>> AddJob(string id, string ten_job);
    }
}