using Microsoft.AspNetCore.Mvc;
using WebBrowser.Interfaces;

namespace WebBrowser.Controllers
{
    [Route("department")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("{id}")]
        public async Task< IActionResult> Getbyid(int id)
        {
            var result = await _departmentService.GetDeptbyid(id);
            return View(result);
        }
    }

}
