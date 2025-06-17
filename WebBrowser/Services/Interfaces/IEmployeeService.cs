using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> getall(string token);
        Task<ApiResponse> Add_emp(string ho_ten, string manv, DateTime ngaysinh, string diachi,
          string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob, string token);
        Task<ApiResponse> DeleteEmp(string manv, string token);
        Task<ApiResponse> UpdateCommision(string manv, string token);
        Task<List<HistoryDto>> GetHistory(string manv, string token);
        Task<ApiResponse> UpdateSalary(string manv, string token);  
    }
}