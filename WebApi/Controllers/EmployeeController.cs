using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using System.DirectoryServices.Protocols;
using WebApi.Handles;
using WebApi.Service.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJobLogicHandler _jobLogicHandler;
        public EmployeeController(IEmployeeService employeeService, IJobLogicHandler jobLogicHandler)
        {
            _employeeService = employeeService;
            _jobLogicHandler = jobLogicHandler;
        }


        [HttpGet("getall")]
        public async Task<IActionResult> getallemp()
        {
            var result= await _employeeService.get_all_emp();
            return Ok(result);
        }


        [HttpPut("UpdateSalary")]
        public async Task<IActionResult> UpdateSalary()
        {
            return await _jobLogicHandler.HandleResponeMessage(()=>_employeeService.UpdateSalary());
        }


        [HttpPut("UpdateCommission")]
        public async Task<IActionResult> UpdateCommision([FromBody] DeleteEmpRequest manv)
        {
            return await _jobLogicHandler.HandleResponeMessage(()=>_employeeService.UpdateCommision(manv.manv));
        }


        [HttpPost("HisEmp")]
        public async Task<IActionResult> HisEmp([FromBody] DeleteEmpRequest em)
        {
            return await _jobLogicHandler.HandleResponeMessage(() => _employeeService.HisEmp(em.manv));
        }



        [HttpPost("add_emp")]
        public async Task<IActionResult> Add_emp(
          [FromBody] Employee employee)
        {
            return await _jobLogicHandler.HandleResponeMessage(() => _employeeService.Add_emp(
            employee.HO_TEN,
            employee.MANV,
            employee.NGSINH ?? DateTime.MinValue,    // hoặc throw nếu không được null
            employee.DCHI,
            employee.PHAI,
            employee.LUONG ?? 0f,
            employee.MA_NQL,
            employee.MAPHG ?? 0,
            employee.NGAY_VAO ?? DateTime.MinValue,
            employee.HOAHONG ?? 0f,
            employee.MAJOB ?? ""
        ));

        }


        [HttpDelete("delete")]
        public async Task<IActionResult> Delete_emp([FromBody] DeleteEmpRequest manv)
        {
            return await _jobLogicHandler.HandleResponeMessage(() => _employeeService.DeleteEmp(manv.manv));
        }
    }
}
