using Microsoft.AspNetCore.Mvc;

namespace CommonLib.unuse
{
    public interface IJobLogicHandler
    {
        Task<IActionResult> HandleResponeMessage(Func<Task<List<Dictionary<string, object>>>> addJobFunc);
        Task<IActionResult> HandleResponseAsync(Func<Task<List<Dictionary<string, object>>>> serviceMethod);
    }
}