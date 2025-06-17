using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Http; // để dùng HttpContext.Session
using System.Threading.Tasks;

namespace WebBrowser.Controllers
{
    [Route("department")]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchById(int id)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _departmentService.GetDeptbyid(id, token);

            if (result == null || result.Count == 0)
            {
                ViewBag.Message = $"Không tìm thấy phòng ban với mã {id}.";
            }

            return View("Getall", result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Getbyid(int id)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _departmentService.GetDeptbyid(id, token);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _departmentService.GetAll(token);
            return View(result);
        }
    }
}