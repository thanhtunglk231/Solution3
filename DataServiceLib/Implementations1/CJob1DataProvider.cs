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

namespace DataServiceLib.Implementations1
{
    public class CJob1DataProvider : ICJob1DataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;

        public CJob1DataProvider(ICBaseDataProvider1 cBaseDataProvider1, IConfiguration configuration, IErrorHandler errorHandler)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
        }

        public DataSet GetAll()
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

                return _dataProvider.GetDatasetFromSP(SpRoute.sp_getall_job, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        public CResponseMessage1 Deletejob(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Deletejob));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_majob", OracleDbType.Char) { Value = manv }
                };

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_delete_job, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi xoá nhân viên" };
            }
        }

        public CResponseMessage1 Addjob(Addjob addjob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CJob1DataProvider), nameof(Addjob));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_majob", OracleDbType.Char) { Value = addjob.MAJOB },
                    new OracleParameter("v_tenjob", OracleDbType.NVarchar2) { Value = addjob.TENJOB }
                };

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_add_hob, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi thêm job" };
            }
        }
    }
}
