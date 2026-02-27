using CoreLib.Models;

namespace DataServiceLib.unuse.Interfaces.unuse
{
    public interface ICEmployeeDataProvider
    {
        Task<List<Dictionary<string, object>>> Add_emp(Employee employee);
        Task<List<Dictionary<string, object>>> DeleteEmp(string manv);
        Task<List<Dictionary<string, object>>> get_all_emp();
        Task<CResponseMessage> HisEmp(string manv);
        Task<List<Dictionary<string, object>>> UpdateCommision(string manv);
        Task<List<Dictionary<string, object>>> UpdateSalary();
    }
}