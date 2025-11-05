using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Dep1Controller : ControllerBase
    {
        private readonly ICDepartmentDataProvider1 _departmentService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;
        public Dep1Controller(ICDepartmentDataProvider1 departmentService,IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _departmentService = departmentService;
            _dataConvertHelper = dataConvertHelper;
            this._errorHandler = errorHandler;
        }


      
        [HttpGet("getalldataset")]
        [CustomAuthorize("admin , user",View = true)]
        public async Task< DataSet> GetDataSet()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "GetDataSet");
                return await _departmentService.GetDataSet();
            }
            catch (Exception ex) { 
            
            _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }
        [HttpGet("getbyiddataset")]
        [CustomAuthorize("admin", View = true)]
        public async Task< DataSet> GetDataSetByid([FromQuery]int id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "GetDataSetByid");
                return await _departmentService.GetbyidDataset(id);
            }
            catch(Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string maphg)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "Delete");

                var result = await _departmentService.Delete(maphg); 

                return Ok(result); 
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "Create");

                var result = await _departmentService.Create(department); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "Update");

                var result = await _departmentService.Update(department); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }



    }
}
