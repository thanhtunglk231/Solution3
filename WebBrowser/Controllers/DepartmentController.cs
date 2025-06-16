using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;

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
        [HttpGet("search")]
        public async Task<IActionResult> SearchById(int id)
        {
            var result = await _departmentService.GetDeptbyid(id);

            if (result == null || result.Count == 0)
            {
                ViewBag.Message = $"Không tìm thấy phòng ban với mã {id}.";
            }

            return View("Getall", result); // View chính danh sách
        }

        [HttpGet("{id}")]
        public async Task< IActionResult> Getbyid(int id)
        {
            var result = await _departmentService.GetDeptbyid(id);

            return View(result);
        }
            [HttpGet]
            public async Task<IActionResult> Getall()
            {
                var result = await _departmentService.getall();
                return View(result);
            }
    }

}
