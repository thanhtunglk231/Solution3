using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class JobController : BaseController
    {
        private readonly IJobService _jobService;
        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }
        public IActionResult AddJob()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddJob(string id, string jobname)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;
            var result = await _jobService.Addjob(id, jobname, token);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["message"] = "Bạn cần đăng nhập";
                return RedirectToAction("Login", "Login");
            }
            var result = await _jobService.getall(token);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Deletejob(string id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["message"] = "Bạn cần đăng nhập";
                return RedirectToAction("Login", "Login");
            }
            var result = await _jobService.DeleteJob(id, token);

            ViewBag.message = result.Message;
            ViewBag.Success = result.Success;

            return RedirectToAction("getall"); 
        }

    }
}
