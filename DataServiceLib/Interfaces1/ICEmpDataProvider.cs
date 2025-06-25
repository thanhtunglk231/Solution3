using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICEmpDataProvider
    {
        CResponseMessage1 AddEmp(Employee emp);
        CResponseMessage1 DeleteEmp(string manv);
        DataSet GetAll();
        DataSet GetHistoryByManv(string manv);
        CResponseMessage1 UpdateCommission(string manv);
        CResponseMessage1 UpdateSalary();
    }
}