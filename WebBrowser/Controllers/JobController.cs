using Microsoft.AspNetCore.Mvc;
using WebBrowser.Interfaces;

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
            ViewBag.message = result.Message;
            ViewBag.IsSuccess = result.Success;
            return View();
        }
    }
}
