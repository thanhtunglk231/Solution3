using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class JobController : Controller
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

            var result = await _jobService.Addjob(id, jobname);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var result = await _jobService.getall();
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Deletejob(string id)
        {
            var result = await _jobService.DeleteJob(id);

            ViewBag.message = result.Message;
            ViewBag.Success = result.Success;

            return RedirectToAction("getall"); 
        }

    }
}
