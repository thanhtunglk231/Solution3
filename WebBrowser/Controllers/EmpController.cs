using CommonLib.Handles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class EmpController : BaseController
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IEmpService _empService;
        public EmpController(IErrorHandler errorHandler, IEmpService empService, IConfiguration configuration) : base(configuration)
        {
            _errorHandler = errorHandler;
            this._empService = empService;
        }
        public IActionResult Index()
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            Console.WriteLine("token" + token);
            if (token == null)
                return redirectResult!;
            return View();
        }
        public async Task<IActionResult> getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "getall");
                var resut = await _empService.GetAllFromDataSet();
                return Json(new { success = true, data = resut });
            }catch(Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = ex });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetHistory(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "gethistory");
                var result = await _empService.GetJobHistory(manv);
                return Json(new { success = true, data= result});
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi lấy lịch sử nhân viên" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "add");
                var result = await _empService.AddEmployee(employee);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi thêm nhân viên" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "delete");
                var result = await _empService.DeleteEmployee(manv);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi xóa nhân viên" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "updatesalary");
                var result = await _empService.UpdateSalary();
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi cập nhật lương" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCommission(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpController", "udatecommission");
                var result = await _empService.UpdateCommission(manv);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi cập nhật hoa hồng" });
            }
        }
    }
}
