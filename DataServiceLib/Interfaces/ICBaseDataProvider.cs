using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces
{
    public interface ICBaseDataProvider
    {
        void CloseConnection();
        Task<List<Dictionary<string, object>>> GetDataSetSP(string spName, IDbDataParameter[] parameters, string connectString);
        Task Executenonqery(string spName, IDbDataParameter[] parameters, string connectionString);
        bool OpenConnection(string connectString);
        Task<CResponseMessage> GetResponseMessage(string SpName, IDbDataParameter[] parameters, string connectionString);
        Task<(List<Dictionary<string, object>> Data, CResponseMessage Response)> GetDataSetAndResponse(string spName, IDbDataParameter[] parameters, string connectionString);
    }
}