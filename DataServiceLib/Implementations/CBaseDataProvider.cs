using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace DataServiceLib.Implementations
{
    public class CBaseDataProvider : ICBaseDataProvider
    {
        private OracleConnection con;
        private OracleCommand command;
        private OracleDataReader reader;
        public string _connectionString;

        public CBaseDataProvider() { }

        public bool OpenConnection(string connectString)
        {
            try
            {
                if (con == null)
                {
                    con = new OracleConnection(connectString);
                }
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("OpenConnection error: " + ex.Message);
                return false;
            }
        }

        public void CloseConnection()
        {
            if (con != null && con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
                con = null;
            }
        }

        public DataSet GetDataSetSP(string spName, IDbDataParameter[] parameters, string connectString)
        {
            DataSet ds = new DataSet();

            if (!OpenConnection(connectString))
                return null;

            try
            {
                using (command = new OracleCommand(spName, con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDataSetSP error: " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }

            return ds;
        }
    }
}
