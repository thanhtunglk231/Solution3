using System.Data;
using CommonLib.Implementations;

using CoreLib.config;
using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using WebApi.Service.Interfaces;
namespace WebApi.Service.Implement
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ICBaseDataProvider _dataProvider;
        private readonly string _connectString;
        private readonly DataTableHelper _tableHelper;
        public DepartmentService(ICBaseDataProvider dataProvider, IConfiguration configuration,DataTableHelper dataTable)
        {
            _dataProvider = dataProvider;
            _tableHelper= dataTable;
            _connectString = configuration.GetConnectionString("OracleDb");

        }
        public async Task<List<Dictionary<string, object>>> getbyid(int id)
        {
            IDbDataParameter[] parameters = new IDbDataParameter[] {
        new OracleParameter("v_maphg", OracleDbType.Int32) { Value = id },
        new OracleParameter("o_cursor", OracleDbType.RefCursor, ParameterDirection.Output),
        };

            var result = await _dataProvider.GetDataSetSP(SpRoute.sp_dept_info, parameters, _connectString);
            return result;
        }

    }
}
