using System.Data;

namespace DataServiceLib.Interfaces
{
    public interface ICBaseDataProvider
    {
        void CloseConnection();
        DataSet GetDataSetSP(string spName, IDbDataParameter[] parameters, string connectString);
        bool OpenConnection(string connectString);
    }
}