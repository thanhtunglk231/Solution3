using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Dtos;
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

                // Khai báo các biến OUT từ procedure
                var o_password = new OracleParameter("o_password", OracleDbType.Varchar2, 400)
                {
                    Direction = ParameterDirection.Output
                };

                var o_role = new OracleParameter("o_role", OracleDbType.Varchar2, 20)
                {
                    Direction = ParameterDirection.Output
                };

                var o_manv = new OracleParameter("o_manv", OracleDbType.Char, 9)
                {
                    Direction = ParameterDirection.Output
                };

                var o_email = new OracleParameter("o_email", OracleDbType.Varchar2, 100)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new IDbDataParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = username
                    },
                    o_password,
                    o_role,
                    o_manv,
                    o_email
                };

                var (ds, response) = _dataProvider.GetDatasetAndResponseFromSP(SpRoute.sp_login, parameters, _connectString);


                if (response.code != "200")
                {
                    return new CResponseMessage1
                    {
                        code = response.code,
                        message = response.message ?? "Không lấy được dữ liệu người dùng",
                        Success = false
                    };
                }

                var hashFromDb = o_password.Value?.ToString()?.Trim();
                var role = o_role.Value?.ToString();
                var manv = o_manv.Value?.ToString();
                var email = o_email.Value?.ToString();

              

                if (string.IsNullOrWhiteSpace(hashFromDb) || !BCrypt.Net.BCrypt.Verify(password.Trim(), hashFromDb))
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
                    Data = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "role", role },
                    { "manv", manv },
                    { "email", email }
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

        public async Task<CResponseMessage1> GetEmail(string input)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(GetEmail));

               

                var o_email = new OracleParameter("o_email", OracleDbType.Varchar2, 100)
                {
                    Direction = ParameterDirection.Output
                };

                var o_username = new OracleParameter("o_username", OracleDbType.Varchar2, 100)
                {
                    Direction = ParameterDirection.Output
                };

                var o_role = new OracleParameter("o_role", OracleDbType.Varchar2, 50)
                {
                    Direction = ParameterDirection.Output
                };

                var o_manv = new OracleParameter("o_manv", OracleDbType.Varchar2, 50)
                {
                    Direction = ParameterDirection.Output
                };

                var p_input = new OracleParameter("p_input", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = input
                };

                var parameters = new IDbDataParameter[]
                {
                    p_input,
                    o_email,
                    o_username,
                    o_role,
                    o_manv
                };

            

                var result = _dataProvider.GetResponseFromExecutedSP("sp_get_email_by_username_or_email", parameters, _connectString);

                var code = result.code?.Trim();
                var msg = result.message?.Trim();

                var email = o_email.Value?.ToString()?.Trim();
                var username = o_username.Value?.ToString()?.Trim();
                var role = o_role.Value?.ToString()?.Trim();
                var manv = o_manv.Value?.ToString()?.Trim();

                
                return new CResponseMessage1
                {
                    code = code,
                    message = msg,
                    Success = code == "200",
                    Data = new
                    {
                        email = email,
                        username = username,
                        role = role,
                        manv = manv
                    }
                };
            }
            catch (Exception ex)
            {
              
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống khi lấy email",
                    Success = false
                };
            }
        }






        public async Task<CResponseMessage1> Register(RegisterDto dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CLoginProvider), nameof(Register));

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

                var para = new IDbDataParameter[]
                {
                    new OracleParameter("v_username", OracleDbType.Varchar2) { Value = dto.username },
                    new OracleParameter("v_password", OracleDbType.Varchar2) { Value = hashedPassword },
                    new OracleParameter("v_email", OracleDbType.Varchar2)
                {
                Value = string.IsNullOrEmpty(dto.email) ? DBNull.Value : dto.email
                }
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



    }
}
