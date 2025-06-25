using CoreLib.Dtos;

namespace WebBrowser.Services.Interfaces
{
    public interface IDepartmentMvcService
    {
       
       
        Task<List<DepartmentDto>> GetAllFromDataSet();
        Task<List<DepartmentDto>> GetbyidDataset(int id);
    }
}