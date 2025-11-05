using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICDepartmentDataProvider1
    {
        Task<CResponseMessage1> Create(Department department);
        Task<CResponseMessage1> Delete(string maphg);
        Task<DataTable> GetAll();
        Task<(DataRow DataRow, CResponseMessage1 Response)> GetById(int id);
        Task<DataSet> GetbyidDataset(int id);
        Task<DataSet> GetDataSet();
        Task<CResponseMessage1> Update(Department department);
    }
}