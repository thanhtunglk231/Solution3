namespace WebApi.Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Dictionary<string, object>>> UpdateCommision(string manv);
        Task<List<Dictionary<string, object>>> Add_emp(
         string ho_ten, string manv, DateTime ngaysinh, string diachi,
         string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob);
        Task<List<Dictionary<string, object>>> get_all_emp();
        Task<List<Dictionary<string, object>>> DeleteEmp(string manv);
        Task<List<Dictionary<string, object>>> UpdateSalary();
        Task<List<Dictionary<string, object>>> HisEmp(string manv);
    }
}