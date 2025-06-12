using CoreLib.Models;
using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public async Task Executenonqery(string spName, IDbDataParameter[] parameters,string connectionString)
        {
            using(var con = new OracleConnection(connectionString))
            {
                
                using(var cmd = new OracleCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach(var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Dictionary<string, object>>> GetDataSetSP(string spName, IDbDataParameter[] parameters, string connectString)
        {
            var lis = new List<Dictionary<string, object>>();

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

                    using (reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                // Cách 1: Bỏ key nếu null
                                //if (value != null)
                                //{
                                //    row[reader.GetName(i)] = value;
                                //}

                                //Cách 2(nếu bạn muốn giữ key và để value là null):
                                 row[reader.GetName(i)] = value;
                            }

                            lis.Add(row);
                        }
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
            return lis;
        }

        public async Task<CResponseMessage> GetResponseMessage(string SpName, IDbDataParameter[] parameters, string connectionString)
        {
            var code = new OracleParameter("o_code", OracleDbType.NVarchar2, 10)
            {
                Direction = ParameterDirection.Output
            };
            var message = new OracleParameter("o_message", OracleDbType.NVarchar2, 200)
            {
                Direction = ParameterDirection.Output
            };

            using (var con = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(SpName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                cmd.Parameters.Add(code);
                cmd.Parameters.Add(message);

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return new CResponseMessage
                {
                    code = code.Value?.ToString(),
                    message = message.Value?.ToString()
                };
            }
        }




    }
}
