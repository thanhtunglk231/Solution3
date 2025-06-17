using DataServiceLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Handles;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly ICDepartmentDataProvider _departmentService;
        private readonly IJobLogicHandler _logicHandler;
        public DepartmentController(ICDepartmentDataProvider departmentService, IJobLogicHandler jobLogicHandler)
        {
            _logicHandler = jobLogicHandler;
            _departmentService = departmentService;
        }
        [HttpGet("getall")]
        public async Task<IActionResult> Get()
        {
            var result = await _departmentService.getall();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _departmentService.getbyid(id);


            return Ok(table);
        }
    }
}