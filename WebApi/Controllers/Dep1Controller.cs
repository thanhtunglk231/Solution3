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

       

    }
}
