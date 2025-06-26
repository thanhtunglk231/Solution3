using CommonLib.Handles;
using CommonLib.Helpers;
using CommonLib.Interfaces;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public DataSet GetAll()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(GetAll));
                var ds = _jobservice.GetAll(); 
                return ds;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        [HttpDelete("delete/{majob}")]
        public IActionResult Delete(string majob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Delete));
                var result = _jobservice.Deletejob(majob);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi xóa Job");
            }
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Addjob addjob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Add));
                var result = _jobservice.Addjob(addjob);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi thêm Job");
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Job job)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(Job1Controller), nameof(Update));
                var result = _jobservice.Update(job);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi cập nhật Job");
            }
        }
    }
}
