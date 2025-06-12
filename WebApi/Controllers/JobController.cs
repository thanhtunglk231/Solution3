
using CoreLib.config;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
using WebApi.Handles;
using WebApi.Service.Interfaces;

    namespace WebApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class JobController : ControllerBase
        {

            private readonly IJobService _jobService;
        private readonly IJobLogicHandler _jobLogicHandler;
            public JobController(IJobService jobService,IJobLogicHandler jobLogicHandler) { 
            _jobService = jobService;
            _jobLogicHandler = jobLogicHandler;
            }
            [HttpPost("addjob")]
            public async Task<IActionResult>AddJob(string id, string jobName)
            {

                 return await _jobLogicHandler.HandleResponeMessage(() => _jobService.AddJob(id,jobName));
            }
        }
    }
