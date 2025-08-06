using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBrowser.Models;

namespace WebBrowser.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;

            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
