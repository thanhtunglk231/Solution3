using CommonLib.Handles;
using CommonLib.Helpers;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Attributes;

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
        public EmpController(ICEmpDataProvider EmpmentService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _empService = EmpmentService;

            _dataConvertHelper = dataConvertHelper;
            this._errorHandler = errorHandler;
        }


        [HttpGet("getall")]
        [CustomAuthorize("admin, user", View = true)]
        public DataSet Getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "Getall");
                return _empService.GetAll();
            }
            catch (Exception ex) {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }

        }
        [HttpPost("add")]
        [CustomAuthorize("admin", Add = true)]
        public IActionResult AddEmployee([FromBody] Employee emp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "add");
                var result = _empService.AddEmp(emp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi thêm nhân viên");
            }
        }
        [HttpDelete("delete/{manv}")]
        [CustomAuthorize("admin", Delete = true)]
        public IActionResult DeleteEmployee(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "Deleteemp");
                var result = _empService.DeleteEmp(manv);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi xóa nhân viên");
            }
        }
        [HttpPut("update-salary")]
        [CustomAuthorize("admin", Edit = true)]
        public IActionResult UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "UpdateSalary");
                var result = _empService.UpdateSalary();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi cập nhật lương");
            }
        }

        [HttpPut("update-commission/{manv}")]
        [CustomAuthorize("admin", Edit = true)]
        public IActionResult UpdateCommission(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "UpdateCommission");
                var result = _empService.UpdateCommission(manv);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi cập nhật hoa hồng");
            }
        }

        [HttpGet("history/{manv}")]
        [CustomAuthorize("admin, user", View = true)]
        public DataSet GetHistory(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "GetHistory");
                var ds = _empService.GetHistoryByManv(manv);
                return ds;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

    }
}
