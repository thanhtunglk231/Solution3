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
    public class CJob1DataProvider : CBaseDataProvider1, ICJob1DataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;
        private readonly IRedisService _redisService;

        private const string CACHE_KEY_ALL = "Job:all";

        public CJob1DataProvider(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            ISerilogProvider logger,
            IErrorHandler errorHandler,
            IRedisService redisService)
            : base(logger)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
            _redisService = redisService;
        }

        public async Task<CResponseMessage1> Update(Job job)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Update));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_majob", OracleDbType.Char) { Value = job.MAJOB },
                    new OracleParameter("v_TENJOB", OracleDbType.NVarchar2, 100) { Value = job.TENJOB },
                    new OracleParameter("v_MAX_SALARY", OracleDbType.Decimal) { Value = job.MAX_SALARY },
                    new OracleParameter("v_MIN_SALARY", OracleDbType.Decimal) { Value = job.MIN_SALARY }
                };

                var result = _dataProvider.GetResponseFromExecutedSP("update_job", para, _connectString);
                result.Success = result.code == "200";

                if (result.Success)
                {
                    await _redisService.DeleteAsync("Dep:all");
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Update));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<CResponseMessage1> Addjob(Addjob addjob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Addjob));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_majob", OracleDbType.Char) { Value = addjob.MAJOB },
                    new OracleParameter("v_tenjob", OracleDbType.NVarchar2, 100) { Value = addjob.TENJOB }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_add_hob, para, _connectString);
                result.Success = result.code == "200";

                if (result.Success)
                {
                    await _redisService.DeleteAsync("Dep:all");
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Addjob));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<CResponseMessage1> Deletejob(string majob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Deletejob));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_majob", OracleDbType.Char) { Value = majob }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_delete_job, para, _connectString);
                result.Success = result.code == "200";

                if (result.Success)
                {
                    await _redisService.DeleteAsync("Dep:all");
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Deletejob));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<DataSet> GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(GetAll));

               
                var cachedData = await _redisService.GetDataSetAsync(CACHE_KEY_ALL);
                if (cachedData != null)
                {
                    return cachedData;
                }

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var result = _dataProvider.GetDatasetFromSP(SpRoute.sp_getall_job, para, _connectString);

                await _redisService.SetDataSetAsync(CACHE_KEY_ALL, result, TimeSpan.FromMinutes(10));

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(GetAll));
                return new DataSet();
            }
        }
    }
}
