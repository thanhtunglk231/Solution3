using CommonLib.Handles;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CommonLib.unuse
{
    public class JobLogicHandler : ErrorHandler,IJobLogicHandler
    {
        public JobLogicHandler(ISerilogProvider logger) : base(logger) { }
        public async Task<IActionResult> HandleResponseAsync(Func<Task<List<Dictionary<string, object>>>> serviceMethod)
        {
            try
            {
                WriteStringToFile("HandleResponseAsync");
                var result = await serviceMethod();
                var responseData = result.FirstOrDefault();

                var code = responseData?["code"]?.ToString();
                var message = responseData?["message"]?.ToString();
              
                if (code == "200")
                {
                    return new OkObjectResult(new { Success = true, Message = message });
                }
                else
                {
                    return new BadRequestObjectResult(new { Success = false, Message = message });
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex);
                return new ObjectResult(new { Success = false, Message = "Internal Server Error", Detail = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }


        public Task<IActionResult> HandleResponeMessage(Func<Task<List<Dictionary<string, object>>>> addJobFunc)
        {
            return HandleResponseAsync(addJobFunc);
        }
    }
}
