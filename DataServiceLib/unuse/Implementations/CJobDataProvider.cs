using CoreLib.config;
using CoreLib.Models;
using DataServiceLib.unuse.Interfaces.unuse;
using Microsoft.AspNetCore.Mvc;
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
    public class CJobDataProvider : ICJobDataProvider
    {
        private readonly ICBaseDataProvider _dataProvider;
        private readonly string _connectString;
        public CJobDataProvider(ICBaseDataProvider dataProvider, IConfiguration configuration)
        {
            _dataProvider = dataProvider;
            _connectString = configuration.GetConnectionString("OracleDb");
        }
        public async Task<List<Dictionary<string, object>>> getall()
        {
            IDbDataParameter[] parameters = new IDbDataParameter[] {
                    new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                    new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
                };

            var result = await _dataProvider.GetDataSetSP(SpRoute.sp_getall_job, parameters, _connectString);

            return result;
        }
        public async Task<CResponseMessage> DeleteJob(string majob)
        {
            majob = majob.Trim();
            Console.WriteLine(majob);
            IDbDataParameter[] parameters = new IDbDataParameter[] {
            new OracleParameter("v_majob", OracleDbType.Char) { Value = majob }
            };

            
            var response = await _dataProvider.GetResponseMessage(SpRoute.sp_delete_job, parameters, _connectString);

            return response; 
        }

        public async Task<List<Dictionary<string, object>>> AddJob(string id, string ten_job)
        {
            IDbDataParameter[] parameters = new IDbDataParameter[] {
                new OracleParameter("v_majob", OracleDbType.Char) { Value = id },
                new OracleParameter("v_tenjob", OracleDbType.NVarchar2) { Value = ten_job },


             };

            var respone = await _dataProvider.GetResponseMessage(SpRoute.sp_add_hob, parameters, _connectString);

            var result = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "code", respone.code },
                    { "message", respone.message }
                }
            };

            return result;
        }


    }
}
