using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICBaseDataProvider1
    {
        void CloseConnection();
        bool ExecuteSP(string spName, IDbDataParameter[] parameters, string connectionString);
        DataRow GetDataRowFromSP(string spName, IDbDataParameter[] parameters, string connectionString);
        DataSet GetDatasetFromSP(string spName, IDbDataParameter[] parameters, string connectionString);
        DataTable GetDataTableFromSP(string spName, IDbDataParameter[] parameters, string connectionString);
        CResponseMessage1 GetResponseFromExecutedSP(string spName, IDbDataParameter[] parameters, string connectionString);
        bool OpenConnection(string connectionString);
        (DataRow Row, CResponseMessage1 Response) GetDataRowAndResponseFromSP(string spName, IDbDataParameter[] parameters, string connectionString);
        (DataSet Data, CResponseMessage1 Response) GetDatasetAndResponseFromSP(string spName, IDbDataParameter[] parameters, string connectionString);
    }
}