using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;

namespace WebBrowser.Services.Interfaces
{
    public interface IEmpService
    {
        Task<CResponseMessage1> AddEmployee(Employee emp);
        Task<CResponseMessage1  > DeleteEmployee(string manv);
        Task<List<Employee>> GetAllFromDataSet();
        Task<List<HistoryDto>> GetJobHistory(string manv);
        Task<CResponseMessage1> UpdateCommission(string manv);
        Task<CResponseMessage1  > UpdateSalary();
    }
}