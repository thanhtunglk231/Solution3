using System.Data;

namespace WebApi.Service.Interfaces
{
    public interface IDepartmentService
    {
        public DataTable GetById(int id);
    }
}