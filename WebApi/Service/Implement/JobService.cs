using CoreLib.config;
using DataServiceLib.Interfaces;
using Microsoft.OpenApi.Writers;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApi.Service.Interfaces;

namespace WebApi.Service.Implement
{
    public class JobService : IJobService
    {

        private readonly ICBaseDataProvider _dataProvider;
        private readonly string _connectString;
        public JobService(ICBaseDataProvider dataProvider,IConfiguration configuration)
        {
            _dataProvider = dataProvider;
            _connectString = configuration.GetConnectionString("OracleDb");
        }
        public async Task<List<Dictionary<string, object>>> AddJob(string id, string ten_job)
        {
          
                    IDbDataParameter[] parameters = new IDbDataParameter[] {
                new OracleParameter("v_majob", OracleDbType.Char) { Value = id },
                new OracleParameter("v_tenjob", OracleDbType.NVarchar2) { Value = ten_job },
                
            };

            var respone =await _dataProvider.GetResponseMessage(SpRoute.sp_add_hob, parameters, _connectString);

                    // Sau khi thực thi xong, đọc giá trị OUT
                    var result = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "code", respone.code },
                    { "message",respone.message }
                }
            };

            return result;
        }




    }
}
