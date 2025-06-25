using CoreLib.Dtos;
using CoreLib.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IJob1Service
    {
        Task<CResponseMessage1> AddJob(Addjob emp);
        Task<CResponseMessage1> DeleteJob(string manv);
        Task<List<Job>> GetAllFromDataSet();
    }
}