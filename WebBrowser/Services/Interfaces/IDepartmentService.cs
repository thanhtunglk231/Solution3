using CoreLib.Models;

public interface IDepartmentService
{
    Task<List<Department>> GetDeptbyid(int id, string token);
    Task<List<Department>> GetAll(string token);
}