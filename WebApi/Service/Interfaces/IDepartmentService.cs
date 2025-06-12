using System.Data;

namespace WebApi.Service.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Dictionary<string, object>>> getbyid(int id);
    }
}