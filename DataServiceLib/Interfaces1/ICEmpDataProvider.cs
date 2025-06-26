using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICEmpDataProvider
    {
        Task<CResponseMessage1> AddEmp(Employee emp);
        Task<CResponseMessage1> DeleteEmp(string manv);
        DataSet GetAll();
        DataSet GetHistoryByManv(string manv);
        Task<CResponseMessage1> UpdateCommission(string manv);
        Task<CResponseMessage1> UpdateSalary();
    }
}