using CoreLib.config;
using CoreLib.Models;
using DataServiceLib.unuse.Interfaces.unuse;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceLib.unuse.Implementations
{
    public class CEmployeeDataProvider : ICEmployeeDataProvider
    {
        public ICBaseDataProvider _cBaseData;
        public string _connectstring;
        public CEmployeeDataProvider(IConfiguration configuration, ICBaseDataProvider cBaseData)
        {
            _cBaseData = cBaseData;
            _connectstring = configuration.GetConnectionString("OracleDb");
        }
        public async Task<List<Dictionary<string, object>>> Add_emp(Employee employee)
        {
            var para = new OracleParameter[]
            {
                    new OracleParameter("p_ho_ten", OracleDbType.NVarchar2) { Value = employee.HO_TEN },
                    new OracleParameter("p_manv", OracleDbType.Char) { Value = employee.MANV },
                    new OracleParameter("p_ngsinh", OracleDbType.Date) { Value = employee.NGSINH },
                    new OracleParameter("p_dchi", OracleDbType.NVarchar2) { Value = employee.DCHI },
                    new OracleParameter("p_phai", OracleDbType.NVarchar2) { Value = employee.PHAI },
                    new OracleParameter("p_luong", OracleDbType.Decimal) { Value = employee.LUONG },
                    new OracleParameter("p_ma_nql   ", OracleDbType.Char) { Value = string.IsNullOrEmpty(employee.MA_NQL) ? DBNull.Value : employee.MA_NQL },
                    new OracleParameter("p_maphg", OracleDbType.Int32) { Value = employee.MAPHG },
                    new OracleParameter("p_ngay_vao", OracleDbType.Date) { Value = employee.NGAY_VAO },
                    new OracleParameter("p_hoahong", OracleDbType.Decimal) { Value = employee.HOAHONG },
                    new OracleParameter("p_majob", OracleDbType.Char) { Value = employee.MAJOB }
            };

            var result = await _cBaseData.GetResponseMessage("add_emp", para, _connectstring);

            return new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "code", result.code },
                        { "message", result.message }
                    }
                };
        }
        public async Task<List<Dictionary<string, object>>> UpdateSalary()
        {
            var result = await _cBaseData.GetResponseMessage(SpRoute.sp_update_emsala, null, _connectstring);

            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "code", result.code },
                    { "message", result.message }
                }
            };
        }

        public async Task<List<Dictionary<string, object>>> DeleteEmp(string manv)
        {
            var para = new OracleParameter[]
           {
                new OracleParameter("v_manv", OracleDbType.Char)
                {
                    Value = manv,
                    Direction = ParameterDirection.Input
                }
           };

            var result = await _cBaseData.GetResponseMessage(SpRoute.sp_delete_em, para, _connectstring);
            return new List<Dictionary<string, object>> {

                new Dictionary<string, object>
                {
                    { "code", result.code },
                    { "message", result.message }
                }
            };


        }
        public async Task<CResponseMessage> HisEmp(string manv)
        {
            var parameters = new OracleParameter[]
            {
                new OracleParameter("v_manv", OracleDbType.Varchar2)
                {
                    Value = manv,
                },
                new OracleParameter("o_cursor", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                },
                new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                },
                new OracleParameter("o_message", OracleDbType.Varchar2, 200)
                {
                    Direction = ParameterDirection.Output
                }
            };

            var result = await _cBaseData.GetDataSetSP("job_his", parameters, _connectstring);
            var Code = parameters[2].Value?.ToString() ?? "500";
            var Message = parameters[3].Value?.ToString() ?? "Lỗi không xác định";
            return new CResponseMessage
            {
                code = Code,
                message = Message,
                Data = result

            };

        }

        public async Task<List<Dictionary<string, object>>> get_all_emp()
        {
            var para = new OracleParameter[]
            {
                new OracleParameter("p_cursor", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                },
                new OracleParameter("o_code", OracleDbType.NVarchar2, 10)
                   {
                            Direction = ParameterDirection.Output
                   },
                new OracleParameter("o_message", OracleDbType.NVarchar2, 200)
                   {
                            Direction = ParameterDirection.Output
                   }

            };

            var result = await _cBaseData.GetDataSetSP(SpRoute.sp_getall_emp, para, _connectstring);


            return result;

        }


        public async Task<List<Dictionary<string, object>>> UpdateCommision(string manv)
        {
            var para = new OracleParameter[]
            {
                new OracleParameter("v_manv", OracleDbType.Char)
                {
                    Value = manv,
                    Direction = ParameterDirection.Input
                }
            };


            var result = await _cBaseData.GetResponseMessage(SpRoute.sp_update_commision, para, _connectstring);


            return new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "code", result.code },
                        { "message", result.message }
                    }
                };
        }
    }

}
