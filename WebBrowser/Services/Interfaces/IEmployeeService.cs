using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> getall();
        Task<ApiResponse> Add_emp(string ho_ten, string manv, DateTime ngaysinh, string diachi,
          string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob);
        Task<ApiResponse> DeleteEmp(string manv);
        Task<ApiResponse> UpdateCommision(string manv);
        Task<List<HistoryDto>> GetHistory(string manv);
        Task<ApiResponse> UpdateSalary(string manv);
    }
}