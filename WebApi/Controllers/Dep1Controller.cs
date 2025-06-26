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
        public DataSet GetDataSet()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "GetDataSet");
                return _departmentService.GetDataSet();
            }
            catch (Exception ex) { 
            
            _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }
        [HttpGet("getbyiddataset/{id}")]
        [CustomAuthorize("admin", View = true)]
        public DataSet GetDataSetByid(int id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "GetDataSetByid");
                return _departmentService.GetbyidDataset(id);
            }
            catch(Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new DataSet();
            }
        }

        [HttpPost("Create")]
     
        public IActionResult Create(Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "Create");
                return Ok(_departmentService.Create(department));
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }


        [HttpDelete("Delete")]

        public IActionResult Delete(string maphg)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "Delete");
                return Ok(_departmentService.Delete(maphg));
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }
        [HttpPut("update")]
        public IActionResult update(Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Dep1Controller", "update");
                return Ok(_departmentService.Update(department));
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return BadRequest();
            }
        }


    }
}
