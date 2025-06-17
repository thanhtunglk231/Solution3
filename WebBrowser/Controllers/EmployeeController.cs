using Microsoft.AspNetCore.Mvc;
using WebBrowser.Models;
using WebBrowser.Models.ViewModels;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Getall(string? msg, bool? success)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var employees = await _employeeService.getall(token!);
            var response = string.IsNullOrWhiteSpace(msg)
                ? null
                : new ApiResponse { Message = msg, Success = success ?? false };

            var viewModel = new EmployeeListViewModel
            {
                Employees = employees,
                Response = response
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new ApiResponse());
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

                // Trim dữ liệu
                ho_ten = ho_ten?.Trim();
                manv = manv?.Trim();
                diachi = diachi?.Trim();
                phai = phai?.Trim();
                mangql = mangql?.Trim();
                majob = majob?.Trim();

                // Gọi service
                var result = await _employeeService.Add_emp(ho_ten, manv, ngaysinh, diachi, phai, luong, mangql, maph, ngayvao, hoahong, majob, token);

                // Nếu thành công, chuyển hướng về danh sách với thông báo
                if (result.Success)
                {
                    return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
                }

                // Nếu thất bại, trả lại view hiện tại với thông báo
                return View(result);
            }
            catch (Exception ex)
            {
                return View(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi server: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalary(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _employeeService.UpdateSalary(manv, token);
            return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCommision(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _employeeService.UpdateCommision(manv, token);
            return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string manv)
        {
            var token = GetJwtTokenOrRedirect(out var redirect);
            if (redirect != null) return redirect;

            var result = await _employeeService.DeleteEmp(manv, token);
            return RedirectToAction("Getall", new { msg = result.Message, success = result.Success });
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
