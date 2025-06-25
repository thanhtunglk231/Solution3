using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CDepartmentDataProvider1 : CBaseDataProvider1, ICDepartmentDataProvider1
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;

        public CDepartmentDataProvider1(
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

        public async Task<DataTable> GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetAll));

                IDbDataParameter[] parameters = new IDbDataParameter[]
                {
                    new OracleParameter("o_dept", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                var result = _dataProvider.GetDataTableFromSP(SpRoute.sp_get_dept, parameters, _connectString);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetAll));
                return new DataTable();
            }
        }

        public DataSet GetDataSet()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetDataSet));

                IDbDataParameter[] parameters = new IDbDataParameter[]
                {
                    new OracleParameter("o_dept", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                return _dataProvider.GetDatasetFromSP(SpRoute.sp_get_dept, parameters, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetDataSet));
                return new DataSet();
            }
        }

        public DataSet GetbyidDataset(int id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetbyidDataset));

                IDbDataParameter[] parameters = new IDbDataParameter[]
                {
                    new OracleParameter("v_maphg", OracleDbType.Int32) { Value = id },
                    new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                return _dataProvider.GetDatasetFromSP(SpRoute.sp_dept_info, parameters, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetbyidDataset));
                return new DataSet();
            }
        }

        public async Task<(DataRow DataRow, CResponseMessage1 Response)> GetById(int id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetById));

                IDbDataParameter[] parameters = new IDbDataParameter[]
                {
                    new OracleParameter("v_maphg", OracleDbType.Int32) { Value = id },
                    new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                var table = _dataProvider.GetDataTableFromSP(SpRoute.sp_dept_info, parameters, _connectString);

                var response = new CResponseMessage1();

                if (table.Rows.Count > 0 && table.Columns.Contains("THONGBAO"))
                {
                    response.code = "404";
                    response.message = table.Rows[0]["THONGBAO"].ToString();
                    return (null, response);
                }

                if (table.Rows.Count > 0)
                {
                    response.code = "200";
                    response.message = "Thành công";
                    return (table.Rows[0], response);
                }

                response.code = "204";
                response.message = "Không có dữ liệu";
                return (null, response);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                _errorHandler.WriteStringToFuncion(nameof(CDepartmentDataProvider1), nameof(GetById));
                return (null, new CResponseMessage1
                {
                    code = "500",
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }
    }
}
