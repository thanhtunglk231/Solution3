using CommonLib.Interfaces;
using CoreLib.Models;
using CoreLib.config;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using CommonLib.Handles;

namespace DataServiceLib.Implementations1
{
    public class CEmpDataProvider : ICEmpDataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;

        public CEmpDataProvider(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            IErrorHandler errorHandler)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
        }

        public DataSet GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetAll));

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                return _dataProvider.GetDatasetFromSP(SpRoute.sp_getall_emp, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        public CResponseMessage1 AddEmp(Employee emp)
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

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_add_emp, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi thêm nhân viên" };
            }
        }

        public CResponseMessage1 DeleteEmp(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(DeleteEmp));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Char) { Value = manv }
                };

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_delete_em, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi xoá nhân viên" };
            }
        }

        public CResponseMessage1 UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateSalary));

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_update_emsala, null, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi cập nhật lương" };
            }
        }

        public CResponseMessage1 UpdateCommission(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(UpdateCommission));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Char) { Value = manv }
                };

                return _dataProvider.GetResponseFromExecutedSP(SpRoute.sp_update_commision, para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi cập nhật hoa hồng" };
            }
        }

        public DataSet GetHistoryByManv(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CEmpDataProvider), nameof(GetHistoryByManv));

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_manv", OracleDbType.Varchar2) { Value = manv },
                    new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

                return _dataProvider.GetDatasetFromSP("job_his", para, _connectString);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }
    }
}
