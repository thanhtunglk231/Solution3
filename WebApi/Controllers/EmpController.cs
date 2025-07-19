using CommonLib.Handles;
using CommonLib.Helpers;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Attributes;
using CoreLib.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmpController : ControllerBase
    {
        private readonly ICEmpDataProvider _empService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public EmpController(ICEmpDataProvider empService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _empService = empService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }

        [HttpGet("getall")]
        [CustomAuthorize("admin, user", View = true)]
        public async Task<DataSet> GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "GetAll");
                return await _empService.GetAll();
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        [HttpPost("add")]
        [CustomAuthorize("admin", Add = true)]
        public async Task<IActionResult> AddEmployee([FromBody] Employee emp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "AddEmployee");
                var result = await _empService.AddEmp(emp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1 { code = "500", message = "Lỗi server khi thêm nhân viên" });
            }
        }

        [HttpDelete("delete")]
        [CustomAuthorize("admin", Delete = true)]
        public async Task<IActionResult> DeleteEmployee([FromQuery] string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "DeleteEmployee");
                var result = await _empService.DeleteEmp(manv);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1 { code = "500", message = "Lỗi server khi xóa nhân viên" });
            }
        }

        [HttpPut("update-salary")]
        [CustomAuthorize("admin", Edit = true)]
        public async Task<IActionResult> UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "UpdateSalary");
                var result = await _empService.UpdateSalary();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1 { code = "500", message = "Lỗi server khi cập nhật lương" });
            }
        }

        [HttpPut("update-commission")]
        [CustomAuthorize("admin", Add = true)]
        public async Task<IActionResult> UpdateCommission([FromQuery]string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "UpdateCommission");
                var result = await _empService.UpdateCommission(manv);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1 { code = "500", message = "Lỗi server khi cập nhật hoa hồng" });
            }
        }

        [HttpGet("history")]
        [CustomAuthorize("admin, user", View = true)]
        public async Task<DataSet> GetHistory([FromQuery] string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "GetHistory");
                return await _empService.GetHistoryByManv(manv);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }
    }
}
