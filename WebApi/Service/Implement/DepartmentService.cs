using System.Data;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using CoreLib.config;
using DataServiceLib.Interfaces;
using Oracle.ManagedDataAccess.Client;
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
        public DataTable GetById(int id)
        {
            // Mở kết nối
            bool con = _dataProvider.OpenConnection(_connectString); // Sửa: gán kết quả đúng
            if (con)
            {
                Console.WriteLine("Kết nối thành công");
            }
            else
            {
                Console.WriteLine("Kết nối thất bại");
                return null;
            }

            // Khởi tạo tham số
            var para = new OracleParameter[]
            {
        new OracleParameter("v_maphg", OracleDbType.Int32)
        {
            Value = id,
            Direction = ParameterDirection.Input
        },
        new OracleParameter("o_cursor", OracleDbType.RefCursor)
        {
            Direction = ParameterDirection.Output
        }
            };

            // Gọi stored procedure
            var ds = _dataProvider.GetDataSetSP(SpRoute.sp_dept_info, para, _connectString);

            // Đóng kết nối
            _dataProvider.CloseConnection();

            // Trả về bảng kết quả
            if (ds != null && ds.Tables.Count > 0)  
            {
                var dt= ds.Tables[0];
                var result = DataTableHelper.ConvertToList(dt);
            }
            return null;
        }

    }
}
