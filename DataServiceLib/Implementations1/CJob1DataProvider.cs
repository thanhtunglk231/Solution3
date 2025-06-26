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

        public CJob1DataProvider(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            ISerilogProvider logger,
            IErrorHandler errorHandler)
            : base(logger)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
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
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Deletejob));
                return new CResponseMessage1 { code = "500", message = ex.Message, Success = false };
            }
        }

        public  DataSet GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(GetAll));

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var result = _dataProvider.GetDatasetFromSP(SpRoute.sp_getall_job, para, _connectString);
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
