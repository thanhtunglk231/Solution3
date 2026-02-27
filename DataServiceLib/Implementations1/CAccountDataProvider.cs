using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CAccountDataProvider : ICAccountDataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;
        private readonly IRedisService _redisService;

        private const string CACHE_KEY_ALL = "Account:all";
        private const string CACHE_KEY_USERNAME = "Account:username";

        public CAccountDataProvider(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            IErrorHandler errorHandler,
            IRedisService redisService)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
            _redisService = redisService;
        }

        public async Task<CResponseMessage1> DeletePermission(UserPermissionDto permissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CAccountDataProvider), nameof(DeletePermission));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_username", OracleDbType.Varchar2) { Value = permissionDto.username },
                    new OracleParameter("v_permis_code", OracleDbType.Varchar2) { Value = permissionDto.Permission_Code },
                };

                var response = _dataProvider.GetResponseFromExecutedSP("remove_permission", para, _connectString);

                response.Success = response?.code == "200";
                response.code ??= "400";
                response.message ??= "Cập nhật quyền thất bại.";

                if (response.Success)
                {
                    await _redisService.DeleteAsync(CACHE_KEY_ALL);
                    await _redisService.DeleteAsync(CACHE_KEY_USERNAME);
                }

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi hệ thống: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> UpdatePermission(UserPermissionDto permissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CAccountDataProvider), nameof(UpdatePermission));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_username", OracleDbType.Varchar2) { Value = permissionDto.username },
                    new OracleParameter("v_permis_code", OracleDbType.Varchar2) { Value = permissionDto.Permission_Code },
                };

                var response = _dataProvider.GetResponseFromExecutedSP("update_permission", para, _connectString);

                response.Success = response?.code == "200";
                response.code ??= "400";
                response.message ??= "Cập nhật quyền thất bại.";

                if (response.Success)
                {
                    await _redisService.DeleteAsync(CACHE_KEY_ALL);
                    await _redisService.DeleteAsync(CACHE_KEY_USERNAME);
                }

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi hệ thống: " + ex.Message
                };
            }
        }

        public async Task<(DataSet data, CResponseMessage1 response)> getall_userName()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CAccountDataProvider), nameof(getall_userName));

                // ✅ Kiểm tra cache
                var cachedData = await _redisService.GetDataSetAsync(CACHE_KEY_USERNAME);
                if (cachedData != null)
                {
                    return (cachedData, new CResponseMessage1 { code = "200", message = "OK", Success = true });
                }

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                var (data, response) = _dataProvider.GetDatasetAndResponseFromSP("get_all_permission_username", para, _connectString);

                // ✅ Cache kết quả
                await _redisService.SetDataSetAsync(CACHE_KEY_USERNAME, data, TimeSpan.FromMinutes(10));

                return (data, response);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (new DataSet(), new CResponseMessage1 { code = "500", message = ex.Message });
            }
        }

        public async Task<(DataSet data, CResponseMessage1 response)> getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CAccountDataProvider), nameof(getall));

                // ✅ Kiểm tra cache
                var cachedData = await _redisService.GetDataSetAsync(CACHE_KEY_ALL);
                if (cachedData != null)
                {
                    return (cachedData, new CResponseMessage1 { code = "200", message = "OK", Success = true });
                }

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                var (data, response) = _dataProvider.GetDatasetAndResponseFromSP(SpRoute.sp_get_permission_users, para, _connectString);

                // ✅ Cache kết quả
                await _redisService.SetDataSetAsync(CACHE_KEY_ALL, data, TimeSpan.FromMinutes(10));

                return (data, response);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (new DataSet(), new CResponseMessage1 { code = "500", message = ex.Message });
            }
        }
    }
}
