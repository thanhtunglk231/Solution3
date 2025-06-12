using CoreLib.config;
using DataServiceLib.Implementations;
using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.Service.Interfaces;

namespace WebApi.Service.Implement
{
    public class EmployeeService : IEmployeeService
    {
        public ICBaseDataProvider _cBaseData;
        public string _connectstring;
        public EmployeeService(IConfiguration configuration, ICBaseDataProvider cBaseData)
        {
            _cBaseData = cBaseData;
            _connectstring = configuration.GetConnectionString("OracleDb");
        }
        public async Task<List<Dictionary<string, object>>> Add_emp(
          string ho_ten, string manv, DateTime ngaysinh, string diachi,
          string phai, float luong, string mangql, int maph, DateTime ngayvao,float hoahong,string majob)
            {
                var para = new OracleParameter[]
                {
            new OracleParameter("p_ho_ten", OracleDbType.NVarchar2) { Value = ho_ten },
            new OracleParameter("p_manv", OracleDbType.Char) { Value = manv },
            new OracleParameter("p_ngsinh", OracleDbType.Date) { Value = ngaysinh },
            new OracleParameter("p_dchi", OracleDbType.NVarchar2) { Value = diachi },
            new OracleParameter("p_phai", OracleDbType.NVarchar2) { Value = phai },
            new OracleParameter("p_luong", OracleDbType.Decimal) { Value = luong },
            new OracleParameter("p_ma_nql   ", OracleDbType.Char) { Value = string.IsNullOrEmpty(mangql) ? DBNull.Value : mangql },
            new OracleParameter("p_maphg", OracleDbType.Int32) { Value = maph },
            new OracleParameter("p_ngay_vao", OracleDbType.Date) { Value = ngayvao },
            new OracleParameter("p_hoahong", OracleDbType.Decimal) { Value = hoahong },
            new OracleParameter("p_majob", OracleDbType.Char) { Value = majob }
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

        public async Task<List<Dictionary<string,object>>> DeleteEmp(string manv)
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
        public async Task<List<Dictionary<string, object>>> HisEmp(string manv)
        {
            var para = new OracleParameter[]
         {
                new OracleParameter("v_manv", OracleDbType.Char)
                {
                    Value = manv,
                    Direction = ParameterDirection.Input
                },
                 new OracleParameter("o_cursor", OracleDbType.RefCursor)
                {
                    
                    Direction = ParameterDirection.Output
                }
         };
            var result = await _cBaseData.GetResponseMessage(SpRoute.sp_his_emp, para, _connectstring);
            return new List<Dictionary<string, object>> {

                new Dictionary<string, object>
                {
                    { "code", result.code },
                    { "message", result.message }
                }
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
            new OracleParameter("o_code", OracleDbType.NVarchar2, 200)
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
