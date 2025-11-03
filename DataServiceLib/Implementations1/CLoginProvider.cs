using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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

        public async Task<(bool IsEnabled, string SecretBase32, DateTimeOffset? LockedUntil)>
  GetTotpInfoAsync(string username)
        {
            try
            {
                var p_user = new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                };

                var o_is_enabled = new OracleParameter("o_is_enabled", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                var o_secret = new OracleParameter("o_secret", OracleDbType.Varchar2, 256)
                {
                    Direction = ParameterDirection.Output
                };

                var o_locked_until = new OracleParameter("o_locked_until", OracleDbType.TimeStampTZ)
                {
                    Direction = ParameterDirection.Output
                };

                var para = new IDbDataParameter[] { p_user, o_is_enabled, o_secret, o_locked_until };

                var resp = _dataProvider.GetResponseFromExecutedSP("sp_get_totp_info", para, _connectString);
                if ((resp.code ?? "").Trim() != "200")
                    return (false, null, null);

                // ✅ CHỖ GÂY LỖI: ép kiểu OracleDecimal trước khi lấy số
                int enabledInt = 0;
                if (o_is_enabled.Value != null && o_is_enabled.Value != DBNull.Value)
                {
                    var od = (OracleDecimal)o_is_enabled.Value;
                    enabledInt = od.IsNull ? 0 : od.ToInt32();
                }
                bool enabled = (enabledInt == 1);

                string secret = o_secret.Value?.ToString();

                DateTimeOffset? locked = null;
                if (o_locked_until.Value != null && o_locked_until.Value != DBNull.Value)
                {
                    var ts = (OracleTimeStampTZ)o_locked_until.Value;
                    locked = ts.IsNull ? null : ts.Value;
                }

                return (enabled, secret, locked);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (false, null, null);
            }
        }




        public async Task EnableTotpAsync(string username, string secretBase32)
        {
            try
            {
                var p_user = new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                };
                var p_secret = new OracleParameter("p_secret", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = secretBase32
                };

                var para = new IDbDataParameter[] { p_user, p_secret };

                var resp = _dataProvider.GetResponseFromExecutedSP("sp_enable_totp", para, _connectString);

                if ((resp.code ?? "").Trim() != "200")
                    throw new Exception("Enable TOTP failed: " + (resp.message ?? ""));
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }



        public async Task TotpSuccessAsync(string username)
        {
            try
            {
                var p_user = new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                };

                var para = new IDbDataParameter[] { p_user };

                var resp = _dataProvider.GetResponseFromExecutedSP("sp_totp_success", para, _connectString);

                var code = (resp.code ?? "").Trim();
                var msg = (resp.message ?? "").Trim();

                if (code != "200")
                    _errorHandler.WriteStringToFuncion("sp_totp_success", msg);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }



        public async Task<(int FailCount, DateTimeOffset? LockedUntil)>
TotpFailIncreaseAsync(string username, int maxAttempts = 5, int lockMinutes = 10)
        {
            try
            {
                var p_user = new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                };

                var p_max = new OracleParameter("p_max_attempts", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Input,
                    Value = maxAttempts
                };

                var p_lock = new OracleParameter("p_lock_minutes", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Input,
                    Value = lockMinutes
                };

                // ✅ dùng Decimal + đọc OracleDecimal
                var o_fail = new OracleParameter("o_fail_count", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                var o_until = new OracleParameter("o_locked_until", OracleDbType.TimeStampTZ)
                {
                    Direction = ParameterDirection.Output
                };

                var para = new IDbDataParameter[] { p_user, p_max, p_lock, o_fail, o_until };

                var resp = _dataProvider.GetResponseFromExecutedSP("sp_mfa_fail_inc", para, _connectString);

                int fc = 0;
                if (o_fail.Value != null && o_fail.Value != DBNull.Value)
                {
                    var od = (OracleDecimal)o_fail.Value;
                    fc = od.IsNull ? 0 : od.ToInt32();
                }

                DateTimeOffset? locked = null;
                if (o_until.Value != null && o_until.Value != DBNull.Value)
                {
                    var ts = (OracleTimeStampTZ)o_until.Value;
                    locked = ts.IsNull ? null : ts.Value;
                }

                return (fc, locked);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (0, null);
            }
        }


    }
}
