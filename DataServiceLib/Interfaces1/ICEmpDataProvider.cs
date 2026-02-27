using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICEmpDataProvider
    {
        Task<CResponseMessage1> AddEmp(Employee emp);
        Task<CResponseMessage1> DeleteEmp(string manv);
        Task<DataSet> GetAll();
        Task<DataSet> GetHistoryByManv(string manv);
        Task<CResponseMessage1> UpdateCommission(string manv);
        Task<CResponseMessage1> UpdateSalary();
    }
}