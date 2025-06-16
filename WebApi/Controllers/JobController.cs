
using CoreLib.config;
using DataServiceLib.Interfaces;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
using WebApi.Handles;


    namespace WebApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class JobController : ControllerBase
        {

            private readonly ICJobDataProvider _jobService;
        private readonly IJobLogicHandler _jobLogicHandler;
            public JobController(ICJobDataProvider jobService,IJobLogicHandler jobLogicHandler) { 
            _jobService = jobService;
            _jobLogicHandler = jobLogicHandler;
            }
            [HttpPost("addjob")]
            public async Task<IActionResult>AddJob(string id, string jobName)
            {

                var result = await _jobService.AddJob(id, jobName);
                return Ok(result);
            }
        [HttpGet("getall")]
        public async Task<IActionResult> Get()
        {

            var result = await _jobService.getall();
            return Ok(result);


        }
        [HttpDelete("delete")]
        public async Task<IActionResult> delete(string majob)
        {
            var resule = await _jobService.DeleteJob(majob);
            return Ok(resule);

        }
    }
    }
