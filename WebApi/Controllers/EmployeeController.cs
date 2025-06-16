using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using System.DirectoryServices.Protocols;
using WebApi.Handles;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ICEmployeeDataProvider _employeeService;
        private readonly IJobLogicHandler _jobLogicHandler;
        public EmployeeController(ICEmployeeDataProvider employeeService, IJobLogicHandler jobLogicHandler)
        {
            _employeeService = employeeService;
            _jobLogicHandler = jobLogicHandler;
        }


        [HttpGet("getall")]
        public async Task<IActionResult> getallemp()
        {
            var result = await _employeeService.get_all_emp ();
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

        [HttpGet("HisEmp")]
        public async Task<IActionResult> HisEmp([FromQuery] string manv)
        {
            var data = await _employeeService.HisEmp(manv);

            if (data == null )
                return NotFound("Không có dữ liệu");

            return Ok(data);
        }
        [HttpPost("add_emp")]
        public async Task<IActionResult> Add_emp([FromBody] Employee employee)
        {
            return await _jobLogicHandler.HandleResponeMessage(() => _employeeService.Add_emp(employee));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete_emp([FromBody] DeleteEmpRequest empRequest)
        {
            return await _jobLogicHandler.HandleResponeMessage(() => _employeeService.DeleteEmp(empRequest.manv));
        }
    }
}
