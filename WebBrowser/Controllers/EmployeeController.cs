using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;
namespace WebBrowser.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService) { 
        _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _employeeService.getall(token!);
            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(); // Trả về view Add.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Add(
          string ho_ten, string manv, DateTime ngaysinh, string diachi,
          string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob)
        {
            try
            {
                var token = GetJwtTokenOrRedirect(out var redirect);
                if (redirect != null) return redirect;


                ho_ten = ho_ten?.Trim();
                manv = manv?.Trim();
                diachi = diachi?.Trim();
                phai = phai?.Trim();
                mangql = mangql?.Trim();
                majob = majob?.Trim();
                var result = await _employeeService.Add_emp(ho_ten, manv, ngaysinh, diachi, phai, luong, mangql, maph, ngayvao, hoahong, majob,token);
                TempData["message"] = result.Message;
                TempData["isSuccess"] = result.Success;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSalary(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;


            var result = await _employeeService.UpdateSalary(manv,token);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCommision(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result= await _employeeService.UpdateCommision(manv, token);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;


            var result= await _employeeService.DeleteEmp(manv, token);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }
        [HttpGet]
        public async Task<IActionResult> HisEmp(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;


            var result = await _employeeService.GetHistory(manv, token);
            return View(result);
        }

    }
}
