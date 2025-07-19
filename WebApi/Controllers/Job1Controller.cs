using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Job1Controller : ControllerBase
    {
        private readonly ICJob1DataProvider _jobservice;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public Job1Controller(
            ICJob1DataProvider jobDataProvider,
            IDataConvertHelper dataConvertHelper,
            IErrorHandler errorHandler)
        {
            _jobservice = jobDataProvider;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }

        [HttpGet("getall")]
        [CustomAuthorize("admin,user", View = true)]
        public async Task<DataSet> GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(GetAll));
                return await _jobservice.GetAll();
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        [HttpDelete("delete")]
        [CustomAuthorize("admin", Delete = true)]
        public async Task<IActionResult> Delete([FromQuery] string majob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Delete));
                var result = await _jobservice.Deletejob(majob);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1
                {
                    code = "500",
                    Success = false,
                    message = "Lỗi server khi xóa Job"
                });
            }
        }

        [HttpPost("add")]
        [CustomAuthorize("admin,user", Add = true)]
        public async Task<IActionResult> Add([FromBody] Addjob addjob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Add));
                var result = await _jobservice.Addjob(addjob);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1
                {
                    code = "500",
                    Success = false,
                    message = "Lỗi server khi thêm Job"
                });
            }
        }

        [HttpPut("update")]
        [CustomAuthorize("admin,user", Add = true)]
        public async Task<IActionResult> Update([FromBody] Job job)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Update));
                var result = await _jobservice.Update(job);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1
                {
                    code = "500",
                    Success = false,
                    message = "Lỗi server khi cập nhật Job"
                });
            }
        }
    }
}
