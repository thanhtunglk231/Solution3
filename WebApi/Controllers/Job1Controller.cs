using CommonLib.Handles;
using CommonLib.Helpers;
using CommonLib.Interfaces;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
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
        public Job1Controller(ICJob1DataProvider jobDataProvider, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _jobservice = jobDataProvider;

            _dataConvertHelper = dataConvertHelper;
            this._errorHandler = errorHandler;
        }

        [HttpGet("getall")]
        [CustomAuthorize("admin, user", View = true)]
        public DataSet Getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Job1Controller", "getall");
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
        [CustomAuthorize("admin", Delete =true)]
        public IActionResult DeleteEmployee(string majob)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Job1Controller", "Deletteemp");
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
        [CustomAuthorize("admin", Add = true)]
        public IActionResult Add([FromBody] Addjob addjob)
        {

            try
            {
                _errorHandler.WriteStringToFuncion("Job1Controller", "add");
                var result = _jobservice.Addjob(addjob);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, "Lỗi server khi Thêm job");
            }

        }


    }
}
