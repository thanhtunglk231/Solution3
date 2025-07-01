using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICJob1DataProvider
    {
        Task<CResponseMessage1> Addjob(Addjob addjob);
        Task<CResponseMessage1> Deletejob(string manv);
        Task<DataSet> GetAll();
        Task<CResponseMessage1> Update(Job job);
    }
}