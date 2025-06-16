using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Interfaces;
namespace WebBrowser.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService) { 
        _employeeService = employeeService;
        }
      
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var resutt = await _employeeService.getall();
            return View(resutt);
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
                ho_ten = ho_ten?.Trim();
                manv = manv?.Trim();
                diachi = diachi?.Trim();
                phai = phai?.Trim();
                mangql = mangql?.Trim();
                majob = majob?.Trim();
                var result = await _employeeService.Add_emp(ho_ten, manv, ngaysinh, diachi, phai, luong, mangql, maph, ngayvao, hoahong, majob);
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
            var result = await _employeeService.UpdateSalary(manv);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCommision(string manv)
        {
            var result= await _employeeService.UpdateCommision(manv);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string manv)
        {
            var result= await _employeeService.DeleteEmp(manv);
            TempData["message"] = result.Message;
            TempData["isSuccess"] = result.Success;
            return RedirectToAction("getall");
        }
        [HttpGet]
        public async Task<IActionResult> HisEmp(string manv)
        {
            var result = await _employeeService.GetHistory(manv);
            return View(result);
        }

    }
}
