using CoreLib.Models;

public interface IDepartmentService
{
    Task<List<Department>> getall();
    Task<List<Department>> GetDeptbyid(int id);
}