using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.Models;
using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataServiceLib.Implementations
{
    public class CBaseDataProvider : ErrorHandler,ICBaseDataProvider
    {
        private OracleConnection con;
        private OracleCommand command;
        private OracleDataReader reader;
        public string _connectionString;

        public CBaseDataProvider(ISerilogProvider logger) : base(logger) { }

        public bool OpenConnection(string connectString)
        {
            try
            {
                Log.Information("OpenConnection");
                if (con == null)
                {
                    con = new OracleConnection(connectString);
                }
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    Log.Information("[OpenConnection] Connection opened successfully.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("[OpenConnection] Failed to open connection.");
                this.WriteToFile(ex);

                return false;
            }

        }

        public void CloseConnection()
        {
            try
            {


                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con = null;
                    Logger.Information("[OpenConnection] Connection opened successfully.");
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex, "[CloseConnection] Failed to open connection.");
                this.WriteToFile(ex);

            }
        }
        public async Task Executenonqery(string spName, IDbDataParameter[] parameters,string connectionString)
        {
            try
            {
                Log.Information("Before", spName);
                using (var con = new OracleConnection(connectionString))
                {

                    using (var cmd = new OracleCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex) {
                this.WriteToFile(ex);
            }
        }

        public async Task<List<Dictionary<string, object>>> GetDataSetSP(string spName, IDbDataParameter[] parameters, string connectString)
        {

            var lis = new List<Dictionary<string, object>>();

            if (!OpenConnection(connectString))
                return new List<Dictionary<string, object>>();

            try
            {
                this.Logger.Information($"Before {spName}");
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

                                //Cách 2(nếu muốn giữ key và để value là null):
                                 row[reader.GetName(i)] = value;
                            }

                            lis.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"GetDataSetSP +{spName}");
                this.WriteToFile(ex);
                return new List<Dictionary<string, object>>();
            }
            finally
            {
                CloseConnection();
            }
            return lis;
        }
        public async Task<(List<Dictionary<string, object>> Data, CResponseMessage Response)> GetDataSetAndResponse(
    string spName, IDbDataParameter[] parameters, string connectionString)
        {
            var response = new CResponseMessage();
            var data = new List<Dictionary<string, object>>();

            try
            {
                Log.Information("Before", spName);

                using var con = new OracleConnection(connectionString);
                using var cmd = new OracleCommand(spName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var codeParam = parameters.FirstOrDefault(p => p.ParameterName == "o_code");
                var msgParam = parameters.FirstOrDefault(p => p.ParameterName == "o_message");

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    data.Add(row);
                }

                if (codeParam != null)
                    response.code = cmd.Parameters["o_code"].Value?.ToString();

                if (msgParam != null)
                    response.message = cmd.Parameters["o_message"].Value?.ToString();

                Log.Information(" SP {SP} hoàn tất với {Count} dòng", spName, data.Count);
            }
            catch (Exception ex)
            {
                Log.Error(" SP {SP} hoàn tất với {Count} dòng", " SP {SP} lỗi", spName);
                //this.WriteToFile(ex);
            }

            return (data, response); 
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
                try
                {
                Log.Information("Before calling stored procedure: {SpName}", SpName);
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
                    Log.Information("Stored Procedure returned: code={Code}, message={Message}", code.Value, message.Value);

                    return new CResponseMessage
                        {
                            code = code.Value?.ToString(),
                            message = message.Value?.ToString()
                        };

                    }
                }
                catch (Exception ex) {
                    this.WriteToFile(ex);
                    return new CResponseMessage
                    {
                        code = "500",
                        message = "Lỗi hệ thống"
                    };
                }
            }

        //public async Task<CResponseMessage> GetResponseMessageWithData(string SpName, OracleParameter[] parameters, string connectionString)
        //{
        //    var code = new OracleParameter("o_code", OracleDbType.NVarchar2, 10)
        //    {
        //        Direction = ParameterDirection.Output
        //    };
        //    var message = new OracleParameter("o_message", OracleDbType.NVarchar2, 200)
        //    {
        //        Direction = ParameterDirection.Output
        //    };

        //    var cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
        //    {
        //        Direction = ParameterDirection.Output
        //    };

        //    using (var con = new OracleConnection(connectionString))
        //    using (var cmd = new OracleCommand(SpName, con))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        if (parameters != null)
        //        {
        //            foreach (var param in parameters)
        //            {
        //                cmd.Parameters.Add(param);
        //            }
        //        }

        //        cmd.Parameters.Add(cursor);
        //        cmd.Parameters.Add(code);
        //        cmd.Parameters.Add(message);

        //        await con.OpenAsync();

        //        var adapter = new OracleDataAdapter(cmd);
        //        var ds = new DataSet();
        //        adapter.Fill(ds);

        //        return new CResponseMessage
        //        {
        //            code = code.Value?.ToString(),
        //            message = message.Value?.ToString(),
        //            Data = ds
        //        };
        //    }
        //}

            



    }
}
