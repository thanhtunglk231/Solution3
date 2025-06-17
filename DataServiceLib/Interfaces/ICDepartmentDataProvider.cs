using CoreLib.Models;

namespace DataServiceLib.Interfaces
{
    public interface ICDepartmentDataProvider
    {
        Task<List<Dictionary<string, object>>> getall();
        Task<List<Dictionary<string, object>>> getbyid(int id);
    }
}