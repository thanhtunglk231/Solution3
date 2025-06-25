using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICDepartmentDataProvider1
    {
        Task<DataTable> GetAll();
        Task<(DataRow DataRow, CResponseMessage1 Response)> GetById(int id);
        DataSet GetDataSet();
        DataSet GetbyidDataset(int id);
    }
}