using CoreLib.Models;

namespace WebBrowser.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDeptbyid(int id);
    }
}