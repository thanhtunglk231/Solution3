using CommonLib.Handles;
using CoreLib.Dtos;
using CoreLib.Models;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class Job1Controller : BaseController
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IJob1Service _jobService;

        public Job1Controller(IErrorHandler errorHandler, IJob1Service jobService, IConfiguration configuration)
            : base(configuration)
        {
            _errorHandler = errorHandler;
            _jobService = jobService;
        }

        public IActionResult Index()
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            Console.WriteLine("token" + token);

            if (token == null)
                return redirectResult!;

            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            const string func = "GetAll";
            _errorHandler.WriteStringToFuncion("Job1Controller", func);

            try
            {
                var result = await _jobService.GetAllFromDataSet();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi lấy danh sách công việc" });
            }
        }

       
        public async Task<IActionResult> Add([FromBody]Addjob job)
        {
            const string func = "Add";
            _errorHandler.WriteStringToFuncion("Job1Controller", func);

            try
            {
                var result = await _jobService.AddJob(job);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi thêm công việc" });
            }
        }
        public async Task<IActionResult> Update([FromBody]Job job)
        {
            const string func = "Update";
            _errorHandler.WriteStringToFuncion("Job1Controller", func);

            try
            {
                var result = await _jobService.Update(job);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi thêm công việc" });
            }
        }

        public async Task<IActionResult> Delete(string majob)
        {
            const string func = "Delete";
            _errorHandler.WriteStringToFuncion("Job1Controller", func);

            try
            {
                var result = await _jobService.DeleteJob(majob);
                return Json(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new { success = false, message = "Lỗi khi xóa công việc" });
            }
        }
    }
}
