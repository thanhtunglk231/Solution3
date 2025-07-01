using CommonLib.Interfaces;
using CoreLib.Models;
using CoreLib.config;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using CommonLib.Handles;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CEmpDataProvider : CBaseDataProvider1, ICEmpDataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;
        private readonly IRedisService _redisService;
        public CEmpDataProvider(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            ISerilogProvider logger,
            IErrorHandler errorHandler,
            IRedisService redisService  )
            : base(logger)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
            _redisService = redisService;
        }

        public async Task<CResponseMessage1> AddEmp(Employee emp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(AddEmp));

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_ho_ten", OracleDbType.NVarchar2) { Value = emp.HO_TEN },
                    new OracleParameter("p_manv", OracleDbType.Char) { Value = emp.MANV },
                    new OracleParameter("p_ngsinh", OracleDbType.Date) { Value = emp.NGSINH ?? (object)DBNull.Value },
                    new OracleParameter("p_dchi", OracleDbType.NVarchar2) { Value = emp.DCHI },
                    new OracleParameter("p_phai", OracleDbType.NVarchar2) { Value = emp.PHAI },
                    new OracleParameter("p_luong", OracleDbType.Decimal) { Value = emp.LUONG ?? (object)DBNull.Value },
                    new OracleParameter("p_ma_nql", OracleDbType.Char) { Value = string.IsNullOrEmpty(emp.MA_NQL) ? DBNull.Value : emp.MA_NQL },
                    new OracleParameter("p_maphg", OracleDbType.Int32) { Value = emp.MAPHG ?? (object)DBNull.Value },
                    new OracleParameter("p_ngay_vao", OracleDbType.Date) { Value = emp.NGAY_VAO ?? (object)DBNull.Value },
                    new OracleParameter("p_hoahong", OracleDbType.Decimal) { Value = emp.HOAHONG ?? (object)DBNull.Value },
                    new OracleParameter("p_majob", OracleDbType.Char) { Value = emp.MAJOB ?? (object)DBNull.Value }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_add_emp, para, _connectString);
                result.Success = result.code == "200";
                if (result.Success)
                {
                    await _redisService.DeleteAsync("emp:all");
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(AddEmp));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<CResponseMessage1> DeleteEmp(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(DeleteEmp));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Char) { Value = manv }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_delete_em, para, _connectString);
                result.Success = result.code == "200";
                if (result.Success)
                {
                    await _redisService.DeleteAsync("emp:all");
                    await _redisService.DeleteAsync($"emp:history:{manv}");
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(DeleteEmp));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<CResponseMessage1> UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateSalary));

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_update_emsala, null, _connectString);
                result.Success = result.code == "200";
                if (result.Success)
                {
                    await _redisService.DeleteAsync("emp:all");
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateSalary));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<CResponseMessage1> UpdateCommission(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateCommission));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Char) { Value = manv }
                };

                var result = _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_update_commision, para, _connectString);
                result.Success = result.code == "200";
                if (result.Success)
                {
                    await _redisService.DeleteAsync("emp:all");
                }
                return await Task.FromResult(result);

            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateCommission));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public async Task<DataSet> GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetAll));
                var cacheKey = "emp:all";

               
                var cachedData = await _redisService.GetDataSetAsync(cacheKey);
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

                var ds = _dataProvider.GetDatasetFromSP(SpRoute.sp_getall_emp, para, _connectString);

                // Cache lại Redis
                await _redisService.SetDataSetAsync(cacheKey, ds, TimeSpan.FromMinutes(10));

                return ds;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetAll));
                return new DataSet();
            }
        }


        public async Task<DataSet> GetHistoryByManv(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetHistoryByManv));
                var cacheKey = $"emp:history:{manv}";
                var cachedData = await _redisService.GetDataSetAsync(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Varchar2) { Value = manv },
                    new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var ds = _dataProvider.GetDatasetFromSP("job_his", para, _connectString);

                
                await _redisService.SetDataSetAsync(cacheKey, ds, TimeSpan.FromMinutes(10));

                return ds;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetHistoryByManv));
                return new DataSet();
            }
        }


    }
}
