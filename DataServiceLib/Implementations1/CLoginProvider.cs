using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CLoginProvider : ICLoginProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;

        public CLoginProvider(ICBaseDataProvider1 dataProvider, IConfiguration configuration, IErrorHandler errorHandler)
        {
            _dataProvider = dataProvider;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
        }

        public async Task<CResponseMessage1> Login(string username, string password)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(Login));

                var o_password_hash = new OracleParameter("o_password_hash", OracleDbType.Varchar2, 400) { Direction = ParameterDirection.Output };
                var o_role = new OracleParameter("o_role", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output };
                var o_manv = new OracleParameter("o_manv", OracleDbType.Char, 100) { Direction = ParameterDirection.Output };

                var para = new IDbDataParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2) { Direction = ParameterDirection.Input, Value = username },
                    o_password_hash, o_role, o_manv
                };

                var (row, response) = _dataProvider.GetDataRowAndResponseFromSP(SpRoute.sp_getpassword_users, para, _connectString);

                if (response.code != "200")
                {
                    return new CResponseMessage1
                    {
                        code = response.code,
                        message = response.message ?? "Tài khoản không tồn tại",
                        Success = false
                    };
                }

                var passwordHash = o_password_hash.Value?.ToString()?.Trim();
                var role = o_role.Value?.ToString();
                var manv = o_manv.Value?.ToString();

                if (string.IsNullOrWhiteSpace(passwordHash) || !BCrypt.Net.BCrypt.Verify(password.Trim(), passwordHash))
                {
                    return new CResponseMessage1
                    {
                        code = "401",
                        message = "Mật khẩu không đúng",
                        Success = false
                    };
                }

                return new CResponseMessage1
                {
                    code = "200",
                    message = "Đăng nhập thành công",
                    Success = true,
                    Data = new List<Dictionary<string, object>>()
                    {
                        new Dictionary<string, object>
                        {
                            { "role", role },
                            { "manv", manv }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi đăng nhập",
                    Success = false
                };
            }
        }

        public async Task<CResponseMessage1> Register(string username, string password)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(Register));

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var para = new IDbDataParameter[]
                {
                    new OracleParameter("v_username", OracleDbType.Varchar2) { Value = username },
                    new OracleParameter("v_password", OracleDbType.Varchar2) { Value = hashedPassword }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_resgis, para, _connectString);

                return new CResponseMessage1
                {
                    code = result.code,
                    message = result.message,
                    Success = result.code == "200"
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi đăng ký",
                    Success = false
                };
            }
        }

        public async Task<CResponseMessage1> Getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(Getall));

                var parameters = new IDbDataParameter[]
                {
                    new OracleParameter("p_result", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var ds = _dataProvider.GetDatasetFromSP("sp_get_all_users", parameters, _connectString);
                var data = new List<Dictionary<string, object>>();

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = row.Table.Columns.Cast<DataColumn>()
                            .ToDictionary(col => col.ColumnName, col => row[col]);
                        data.Add(dict);
                    }
                }

                return new CResponseMessage1
                {
                    code = parameters[1].Value?.ToString(),
                    message = parameters[2].Value?.ToString(),
                    Success = parameters[1].Value?.ToString() == "200",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi lấy danh sách người dùng",
                    Success = false
                };
            }
        }

        public async Task<(List<Dictionary<string, object>> data, CResponseMessage1 response)> GetPermission()
        {
            var response = new CResponseMessage1();
            var data = new List<Dictionary<string, object>>();

            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(GetPermission));

                var parameters = new IDbDataParameter[]
                {
            new OracleParameter("v_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
            new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
            new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var ds = _dataProvider.GetDatasetFromSP(SpRoute.sp_get_permission_users, parameters, _connectString);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = row.Table.Columns.Cast<DataColumn>()
                            .ToDictionary(col => col.ColumnName, col => row[col]);
                        data.Add(dict);
                    }
                }

                response.code = parameters[1].Value?.ToString();
                response.message = parameters[2].Value?.ToString();
                response.Success = response.code == "200";
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                response = new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi lấy quyền người dùng",
                    Success = false
                };
            }

            return (data, response);
        }


        public async Task<CResponseMessage1> UpdateUserInfo(string username, string newPassword, string newRole, string newManv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(UpdateUserInfo));

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var para = new IDbDataParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2) { Value = username },
                    new OracleParameter("p_new_password", OracleDbType.Varchar2) { Value = hashedPassword },
                    new OracleParameter("p_new_role", OracleDbType.Varchar2) { Value = newRole },
                    new OracleParameter("p_new_manv", OracleDbType.Varchar2) { Value = newManv }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_update_users, para, _connectString);

                return new CResponseMessage1
                {
                    code = result.code,
                    message = result.message,
                    Success = result.code == "200"
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi cập nhật thông tin người dùng",
                    Success = false
                };
            }
        }
    }
}
