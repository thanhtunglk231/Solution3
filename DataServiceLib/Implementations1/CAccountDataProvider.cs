using CommonLib.Handles;
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
using System.Text;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CAccountDataProvider : ICAccountDataProvider
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;

        public CAccountDataProvider(ICBaseDataProvider1 cBaseDataProvider1, IConfiguration configuration, IErrorHandler errorHandler)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
        }

        public async Task<CResponseMessage1> DeletePermission(UserPermissionDto permissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("CAccountDataProvider", "DeletePermission");

                var para = new OracleParameter[]
                {
                    new OracleParameter("v_username", OracleDbType.Varchar2) { Value = permissionDto.username },
                    new OracleParameter("v_permis_code", OracleDbType.Varchar2) { Value = permissionDto.Permission_Code },
                };

                var response = _dataProvider.GetResponseFromExecutedSP("remove_permission", para, _connectString);

               
                if (response != null && response.code == "200")
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    // Tránh trường hợp null code vẫn bị hiểu là thành công
                    response.code ??= "400";
                    response.message ??= "Cập nhật quyền thất bại.";
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
                _errorHandler.WriteStringToFuncion("CAccountDataProvider", "UpdatePermission");

                var para = new OracleParameter[]
                {
            new OracleParameter("v_username", OracleDbType.Varchar2) { Value = permissionDto.username },
            new OracleParameter("v_permis_code", OracleDbType.Varchar2) { Value = permissionDto.Permission_Code },
                };

                var response = _dataProvider.GetResponseFromExecutedSP("update_permission", para, _connectString);

                // Đảm bảo gán đúng Success nếu thành công
                if (response != null && response.code == "200")
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    // Tránh trường hợp null code vẫn bị hiểu là thành công
                    response.code ??= "400";
                    response.message ??= "Cập nhật quyền thất bại.";
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

            public (DataSet data, CResponseMessage1 response) getall_userName()
            {
                try
                {
                    _errorHandler.WriteStringToFuncion("CAccountDataProvider", "getall_userName");

                    var para = new OracleParameter[]
                    {
                        new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },

                    };
                    var result = _dataProvider.GetDatasetAndResponseFromSP("get_all_permission_username", para, _connectString);
                    return result;
                }
                catch (Exception ex)
                {

                    _errorHandler.WriteToFile(ex);
                    return (new DataSet(), new CResponseMessage1 { code = "500", message = ex.Message });
                }
            }
        public (DataSet data, CResponseMessage1 response) getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("CAccountDataProvider", "getall");

                var para = new OracleParameter[]
                {
                    new OracleParameter("p_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },

                };
                var result = _dataProvider.GetDatasetAndResponseFromSP(SpRoute.sp_get_permission_users, para, _connectString);
                return result;
            }
            catch (Exception ex)
            {

                _errorHandler.WriteToFile(ex);
                return (new DataSet(), new CResponseMessage1 { code = "500", message = ex.Message });
            }
        }
    }
}
