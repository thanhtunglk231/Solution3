using CoreLib.Dtos;
using CoreLib.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IDepartmentMvcService
    {


        Task<CResponseMessage1> Create(Department department);
        Task<List<DepartmentDto>> GetAllFromDataSet();
        Task<List<DepartmentDto>> GetbyidDataset(int id);
        Task<CResponseMessage1> Delete(string ma);
        Task<CResponseMessage1> update(Department department);
    }
}