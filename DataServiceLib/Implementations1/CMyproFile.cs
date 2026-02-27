using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    // Lưu ý: class này kế thừa ErrorHandler để dùng WriteToFile / Logger giống style của bạn
    public class CMyproFile : ErrorHandler
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly IRedisService _redisService;
        private readonly ISerilogProvider _logger;

        public CMyproFile(
            ICBaseDataProvider1 dataProvider,
            IRedisService redisService,
            ISerilogProvider logger
        ) : base(logger)
        {
            _dataProvider = dataProvider;
            _redisService = redisService;
            _logger = logger;
        }

        // ====== ROUTE SP (bạn có SpRoute thì thay vào đây) ======
        private const string SP_GET_MY_PROFILE = "get_my_profile";
        private const string SP_UPDATE_MY_PROFILE = "update_my_profile";
        private const string SP_DELETE_MY_ACCOUNT = "delete_my_account";

        /// <summary>
        /// Lấy thông tin cá nhân: trả DataSet + code/message (cursor + out code/message)
        /// </summary>
        public async Task<(DataSet Data, CResponseMessage1 Response)> GetMyProfile(string username, string connectString)
        {
            try
            {
                _logger.Logger.Information("CMyproFile.GetMyProfile called for {Username}", username);

                var cacheKey = $"myprofile:{username}";
                var cached = await _redisService.GetDataSetAsync(cacheKey);
                if (cached != null)
                {
                    return (cached, new CResponseMessage1 { code = "200", message = "Cache hit." });
                }

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = username
                    },
                    new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    }
                    // o_code, o_message sẽ được base tự add trong GetDatasetAndResponseFromSP
                };

                var result = _dataProvider.GetDatasetAndResponseFromSP(SP_GET_MY_PROFILE, para, connectString);

                // Cache nếu ok
                if (result.Response?.code == "200")
                {
                    await _redisService.SetDataSetAsync(cacheKey, result.Data, TimeSpan.FromMinutes(10));
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteToFile(ex);
                return (new DataSet(), new CResponseMessage1 { code = "500", message = ex.Message });
            }
        }

        /// <summary>
        /// Update thông tin cá nhân: chỉ trả code/message (ExecuteSP + out code/message)
        /// </summary>
        public async Task<CResponseMessage1> UpdateMyProfile(
            string username,
            string? email,
            string? sdt,
            string? manv,
            string connectString)
        {
            try
            {
                _logger.Logger.Information("CMyproFile.UpdateMyProfile called for {Username}", username);

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = username
                    },
                    new OracleParameter("p_email", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = (object?)email ?? DBNull.Value
                    },
                    new OracleParameter("p_sdt", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = (object?)sdt ?? DBNull.Value
                    },
                    new OracleParameter("p_manv", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = (object?)manv ?? DBNull.Value
                    }
                    // o_code, o_message base sẽ tự add
                };

                var res = _dataProvider.GetResponseFromExecutedSP(SP_UPDATE_MY_PROFILE, para, connectString);

                // Clear cache để lần sau GET ra dữ liệu mới
                await _redisService.DeleteAsync($"myprofile:{username}");

                return res;
            }
            catch (Exception ex)
            {
                WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = ex.Message };
            }
        }

        /// <summary>
        /// Xóa tài khoản: chỉ trả code/message (ExecuteSP + out code/message)
        /// </summary>
        public async Task<CResponseMessage1> DeleteMyAccount(string username, string connectString)
        {
            try
            {
                _logger.Logger.Information("CMyproFile.DeleteMyAccount called for {Username}", username);

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_username", OracleDbType.Varchar2)
                    {
                        Direction = ParameterDirection.Input,
                        Value = username
                    }
                    // o_code, o_message base sẽ tự add
                };

                var res = _dataProvider.GetResponseFromExecutedSP(SP_DELETE_MY_ACCOUNT, para, connectString);

                await _redisService.DeleteAsync($"myprofile:{username}");

                return res;
            }
            catch (Exception ex)
            {
                WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = ex.Message };
            }
        }
    }

    
}
