using Microsoft.AspNetCore.Mvc;

namespace WebApi.Handles
{
    public class JobLogicHandler : IJobLogicHandler
    {
        public async Task<IActionResult> HandleResponseAsync(Func<Task<List<Dictionary<string, object>>>> serviceMethod)
        {
            try
            {
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
