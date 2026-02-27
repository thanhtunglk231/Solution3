using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using CoreLib.Models;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class DeptController : BaseController
    {
        private readonly IDepartmentMvcService _departmentService;
        private readonly IErrorHandler _errorHandler;
        private readonly IDataConvertHelper _dataConvertHelper;

        public DeptController(IDepartmentMvcService departmentService, IConfiguration configuration,
                              IErrorHandler errorHandler, IDataConvertHelper dataConvertHelper)
            : base(configuration)
        {
            _departmentService = departmentService;
            _errorHandler = errorHandler;
            _dataConvertHelper = dataConvertHelper;
        }

        public IActionResult Index()
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;
            return View();
        }


        public async Task<IActionResult> Update([FromBody]Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("DeptController", "update");
                var list = await _departmentService.update(department);
                return Json(new { success = true, data = list });
            }
            catch(Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> Create([FromBody]Department department)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("DeptController", "Create");
                var list = await _departmentService.Create(department);
                return Json(new { success = true, data = list });
            }

            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }



       
        public async Task<IActionResult> Delete(string maphg)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("DeptController", "Delete");
                var list = await _departmentService.Delete(maphg);
                return Json(new { success = true, data = list });
            }

            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IActionResult> getalldataset()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("DeptController", "getalldataset");
                var list = await _departmentService.GetAllFromDataSet();
                return Json(new { success = true, data = list });
            }
           
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> getbyiddataset(int id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("DeptController", "getbyiddataset");
                var list = await _departmentService.GetbyidDataset(id);
                return Json(new { success = true, data = list });
            }
           
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý yêu cầu." });
            }
        }


    }
}
