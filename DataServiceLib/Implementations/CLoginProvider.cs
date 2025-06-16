using CoreLib.config;
using CoreLib.Models;
using DataServiceLib.Interfaces;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations
{
    public class CLoginProvider : ICLoginProvider
    {
        private readonly ICBaseDataProvider _dataProvider;
        private readonly string _connectString;
        
        public CLoginProvider(ICBaseDataProvider dataProvider,IConfiguration configuration)
        {
            _dataProvider = dataProvider;
            _connectString = configuration.GetConnectionString("OracleDb");
        }
        public async Task<CResponseMessage> Login(string username, string password)
        {
            Console.WriteLine($"Username nhận được: [{username}]");
            Console.WriteLine($"Password nhận được: [{password}]");
            var o_role = new OracleParameter("o_role", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Output,
                Size = 200
            };

            var o_manv = new OracleParameter("o_manv", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Output,
                Size = 200
            };


            var para = new IDbDataParameter[]
            {
                new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                },
                new OracleParameter("p_password", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = password
                },
                o_role,
                o_manv
            };

            var spname = SpRoute.sp_login;


            var result = await _dataProvider.GetResponseMessage(spname, para, _connectString);
            if (result != null && result.code == "200")
            {
                Console.WriteLine($"Role từ SP: {o_role.Value}, MANV từ SP: {o_manv.Value}");

                result.Data = new List<Dictionary<string, object>>
             {
                new Dictionary<string, object>
                {
                    { "role", o_role.Value?.ToString() ?? string.Empty },
                    { "manv", o_manv.Value?.ToString() ?? string.Empty }
                }
             };
            }
            return result ?? new CResponseMessage { code = "500", message = "Lỗi khác" };
        }


    }
}
