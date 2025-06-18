using Microsoft.AspNetCore.Mvc;
using WebBrowser.Models.ViewModels;
using WebBrowser.Models;
using WebBrowser.Services.Interfaces;
using CoreLib.Helper;

namespace WebBrowser.Controllers
{
    public class JobController : BaseController
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobtService, IConfiguration configuration)
     : base(configuration)
        {
            _jobService = jobtService;
        }

        [HttpGet]
        public async Task<IActionResult> Getall(string? msg, bool? success)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var jobs = await _jobService.getall(token!);
            var response = string.IsNullOrWhiteSpace(msg) ? null : new ApiResponse { Message = msg, Success = success ?? false };

            var vm = new JobListViewModel
            {
                Jobs = jobs,
                Response = response
            };

            return View(vm);
        }

        public IActionResult AddJob()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Getall", new { msg = "Không có quyền truy cập trang thêm công việc", success = false });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddJob(string id, string jobname)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Getall", new { msg = "Không có quyền thêm", success = false });

            var result = await _jobService.Addjob(id.Trim(), jobname.Trim(), token);
            return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
        }

        [HttpPost]
        public async Task<IActionResult> Deletejob(string id)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Getall", new { msg = "Không có quyền xóa", success = false });

            var result = await _jobService.DeleteJob(id, token);
            return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
        }
    }
}
