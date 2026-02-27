using CommonLib.unuse;
using CoreLib.config;
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
    public class CDepartmentDataProvider : ICDepartmentDataProvider
    {
        private readonly ICBaseDataProvider _dataProvider;
        private readonly string _connectString;
        private readonly DataTableHelper _tableHelper;
        public CDepartmentDataProvider(ICBaseDataProvider dataProvider, IConfiguration configuration, DataTableHelper dataTable)
        {
            _dataProvider = dataProvider;
            _tableHelper = dataTable;
            _connectString = configuration.GetConnectionString("OracleDb");

        }
        public async Task<List<Dictionary<string, object>>> getall()
        {
            IDbDataParameter[] parameters = new IDbDataParameter[] {
                new OracleParameter("o_dept", OracleDbType.RefCursor) { Direction = ParameterDirection.Output },
                new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output },
                new OracleParameter("o_message", OracleDbType.Varchar2, 200) { Direction = ParameterDirection.Output }
            };

            var result = await _dataProvider.GetDataSetSP(SpRoute.sp_get_dept, parameters, _connectString);

            return result;
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
